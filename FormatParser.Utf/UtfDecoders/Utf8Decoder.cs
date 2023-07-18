using System.Text;
using FormatParser.Text.Decoders;

namespace FormatParser.Text.UtfDecoders;

public class Utf8Decoder : DecoderBase, IUtfDecoder
{
    private readonly CodepointValidatorSettings settings;

    public Utf8Decoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }

    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) Encoding.UTF8.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override IEnumerable<char> GetInvalidCharacters => InvalidCharacterHelper
        .GetForbiddenChars(settings);

    public override int MinimalSizeOfInput { get; } = 0;

    public override bool SupportBom { get; } = true;
    public override EncodingInfo EncodingWithBom { get; } = EncodingInfo.Utf8BOM;
    public override EncodingInfo EncodingWithoutBom { get; } = EncodingInfo.Utf8NoBOM;

    public override string? RequiredEncodingAnalyzer { get; } = "UTF-8";
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
}