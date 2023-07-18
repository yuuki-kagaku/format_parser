using System.Text;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class Utf16BeDecoder : DecoderBase, ITextDecoder
{
    private readonly CharacterValidatorSettings settings;

    public Utf16BeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed,
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

    protected override int MinimalSizeOfInput => 8;

    protected override bool SupportBom => true;
    protected override EncodingInfo EncodingWithBom => EncodingInfo.Utf16BeBom;
    public override EncodingInfo EncodingWithoutBom => EncodingInfo.Utf16BeNoBom;

    public override string[]? RequiredEncodingAnalyzers { get; } = { "UTF-16" };

public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
}