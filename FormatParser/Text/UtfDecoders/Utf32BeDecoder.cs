using System.Text;
using FormatParser.Text.Encoding;

namespace FormatParser.Text;

public class Utf32BeDecoder : DecoderBase, IUtfDecoder
{
    private readonly CodepointValidatorSettings settings;

    public Utf32BeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (System.Text.Encoding) new UTF32Encoding(true, true, true).Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override HashSet<char> GetInvalidCharacters => CodepointValidator
        .GetForbiddenChars(settings)
        .ToHashSet();
    
    public override int MinimalSizeOfInput { get; } = 8;

    public override bool SupportBom { get; } = true;
    public override string EncodingWithBom { get; } = WellKnownEncodings.UTF32BeBom;
    public override string EncodingWithoutBom { get; } = WellKnownEncodings.UTF32BeNoBom;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
}