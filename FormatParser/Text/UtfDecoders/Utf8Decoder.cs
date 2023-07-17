using System.Text;
using FormatParser.Text.Encoding;

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
        var encoding = (System.Text.Encoding) System.Text.Encoding.UTF8.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override HashSet<char> GetInvalidCharacters => InvalidCharacterHelper
        .GetForbiddenChars(settings)
        .ToHashSet();

    public override int MinimalSizeOfInput { get; } = 0;

    public override bool SupportBom { get; } = true;
    public override EncodingData EncodingWithBom { get; } = EncodingData.Utf8BOM;
    public override EncodingData EncodingWithoutBom { get; } = EncodingData.Utf8NoBOM;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
}