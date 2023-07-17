using System.Text;
using FormatParser.Text.Encoding;

namespace FormatParser.Text.UtfDecoders;

public class Utf16LeDecoder : DecoderBase, IUtfDecoder
{
    private readonly CodepointValidatorSettings settings;

    public Utf16LeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (System.Text.Encoding) System.Text.Encoding.Unicode.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override HashSet<char> GetInvalidCharacters => CodepointValidator
        .GetForbiddenChars(settings)
        .ToHashSet();
    
    public override int MinimalSizeOfInput { get; } = 8;
    
    public override bool SupportBom { get; } = true;
    public override string EncodingWithBom { get; } = WellKnownEncodings.UTF16LeBom;
    public override string EncodingWithoutBom { get; } = WellKnownEncodings.UTF16LeNoBom;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.No;
}