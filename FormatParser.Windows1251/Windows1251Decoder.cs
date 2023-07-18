using System.Text;
using FormatParser.Domain;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Windows1251;

public class Windows1251Decoder : DecoderBase
{
    private readonly CharacterValidatorSettings settings;

    public Windows1251Decoder(TextFileParsingSettings settings) => 
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);

    protected override Decoder GetDecoder(int inputSize) => Decoder;

    private static readonly Decoder Decoder = GetDecoder();

    public override IEnumerable<char> GetInvalidCharacters => InvalidCharacterHelper
        .GetForbiddenChars(settings)
        .Concat(new char[] { (char)152 })
        .ToHashSet();

    private static Decoder GetDecoder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (Encoding)Encoding.GetEncoding("windows-1251").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }

    protected override int MinimalSizeOfInput => 0;

    protected override bool SupportBom => false;
    protected override EncodingInfo EncodingWithBom => throw new NotSupportedException();
    public override EncodingInfo EncodingWithoutBom { get; } = new("Windows-1251", Endianness.NotAllowed, false);

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
    public override string[]? RequiredEncodingAnalyzers { get; } = { "ru" } ;

}