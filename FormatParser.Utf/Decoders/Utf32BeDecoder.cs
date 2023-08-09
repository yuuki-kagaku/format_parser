using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class Utf32BeDecoder : DecoderBase, ITextDecoder
{
    private readonly HashSet<char> invalidChars;

    public Utf32BeDecoder(TextFileParsingSettings settings)
    {
        var characterValidationSettings = new CharacterValidationSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
        invalidChars = InvalidCharactersHelper.GetForbiddenChars(characterValidationSettings).ToHashSet();
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) new UTF32Encoding(true, true, true).Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    protected override IReadOnlySet<char> InvalidCharacters => invalidChars;

    protected override int MinimalSizeOfInput => 8;

    public override bool SupportBom => true;

    public override string EncodingName => WellKnownEncodings.Utf32;
    protected override EncodingInfo EncodingWithBom => WellKnownEncodingInfos.Utf32BeBom;
    protected override EncodingInfo EncodingWithoutBom => WellKnownEncodingInfos.Utf32BeNoBom;

    public override string[]? RequiredEncodingAnalyzers => null;

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.Low;
}