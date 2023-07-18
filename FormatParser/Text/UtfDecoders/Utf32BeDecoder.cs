using System.Text;
using FormatParser.Text.Decoders;

namespace FormatParser.Text.UtfDecoders;

public class Utf32BeDecoder : DecoderBase, IUtfDecoder
{
    private readonly CodepointValidatorSettings settings;

    public Utf32BeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) new UTF32Encoding(true, true, true).Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override IEnumerable<char> GetInvalidCharacters => InvalidCharacterHelper
        .GetForbiddenChars(settings);
    
    public override int MinimalSizeOfInput { get; } = 8;

    public override bool SupportBom { get; } = true;
    public override EncodingInfo EncodingWithBom { get; } = EncodingInfo.UTF32BeBom;
    public override EncodingInfo EncodingWithoutBom { get; } = EncodingInfo.UTF32BeNoBom;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
}