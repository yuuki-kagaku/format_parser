using System.Text;
using FormatParser.Text;
using FormatParser.Text.Encoding;

namespace FormatParser.Windows1251;

public class Windows1251Decoder : DecoderBase
{
    private readonly CodepointValidatorSettings settings;

    public Windows1251Decoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);
    }
    
    protected override Decoder GetDecoder(int inputSize) => Decoder;

    private static readonly Decoder Decoder = GetDecoder();

    public override HashSet<char> GetInvalidCharacters => CodepointValidator
        .GetForbiddenChars(settings)
        .Concat(new char[] { (char)152 })
        .ToHashSet();

    private static Decoder GetDecoder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (Encoding) Encoding.GetEncoding("windows-1251").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }
    
    public override int MinimalSizeOfInput { get; } = 0;
    
    public override bool SupportBom { get; } = false;
    public override EncodingData EncodingWithBom => throw new NotSupportedException();
    public override EncodingData EncodingWithoutBom { get; } = new ("Windows-1251", Endianness.NotAllowed, false);

    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
    public override string? RequiredEncodingAnalyzer { get; } = "ru";
}