using System.Text;
using FormatParser.Text.Decoders;

namespace FormatParser.Text.UtfDecoders;

public class Utf32LeDecoder : DecoderBase, IUtfDecoder
{
    private readonly CodepointValidatorSettings settings;

    public Utf32LeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) new UTF32Encoding(false, true, true).Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override HashSet<char> GetInvalidCharacters => InvalidCharacterHelper
        .GetForbiddenChars(settings)
        .ToHashSet();
    
    public override bool SupportBom { get; } = true;
    public override EncodingInfo EncodingWithBom { get; } = EncodingInfo.UTF32LeBom;
    public override EncodingInfo EncodingWithoutBom { get; } = EncodingInfo.UTF32LeNoBom;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override int MinimalSizeOfInput { get; } = 8;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
}