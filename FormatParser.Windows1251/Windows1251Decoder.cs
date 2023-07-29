using System.Text;
using FormatParser.Domain;
using FormatParser.Helpers;
using FormatParser.Text;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Windows1251;

public class Windows1251Decoder : NonUtfDecoder
{
    private readonly CharacterValidatorSettings settings;

    public Windows1251Decoder(TextFileParsingSettings settings) => 
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);
    
    public override IEnumerable<char> GetInvalidCharacters => InvalidCharactersHelper
        .GetForbiddenChars(settings)
        .Concat((char)152)
        .ToHashSet();

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
    public override string[]? RequiredEncodingAnalyzers { get; } = { "ru" };

    protected override EncodingInfo EncodingInfo { get; } = new("Windows-1251", Endianness.NotAllowed, false);

    protected override Decoder GetDecoder(int inputSize) => GetDecoder();

    protected override int MinimalSizeOfInput => 0;

    private static Decoder GetDecoder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (Encoding)Encoding.GetEncoding("windows-1251").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }
}