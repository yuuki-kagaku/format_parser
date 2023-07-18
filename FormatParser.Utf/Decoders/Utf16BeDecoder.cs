using System.Text;

namespace FormatParser.Text.Decoders;

public class Utf16BeDecoder : DecoderBase, ITextDecoder
{
    private readonly CodepointValidatorSettings settings;

    public Utf16BeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CodepointValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed,
            settings.AllowC1ControlsForUtf, false);
    }

    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding)Encoding.BigEndianUnicode.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override IEnumerable<char> GetInvalidCharacters => InvalidCharacterHelper
        .GetForbiddenChars(settings);

    public override int MinimalSizeOfInput { get; } = 8;

    public override bool SupportBom { get; } = true;
    public override EncodingInfo EncodingWithBom { get; } = EncodingInfo.UTF16BeBom;
    public override EncodingInfo EncodingWithoutBom { get; } = EncodingInfo.UTF16BeNoBom;

    public override string[]? RequiredEncodingAnalyzers { get; } = { "UTF-16" };

public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.No;
}