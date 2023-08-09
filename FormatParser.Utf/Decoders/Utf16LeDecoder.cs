using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class Utf16LeDecoder : DecoderBase, ITextDecoder
{
    private readonly HashSet<char> invalidChars;

    public Utf16LeDecoder(TextFileParsingSettings settings)
    {
        var characterValidationSettings = new CharacterValidationSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
        invalidChars = InvalidCharactersHelper.GetForbiddenChars(characterValidationSettings).ToHashSet();
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) Encoding.Unicode.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    protected override IReadOnlySet<char> InvalidCharacters => invalidChars;

    protected override int MinimalSizeOfInput => 8;

    public override bool SupportBom => true;
    
    public override string EncodingName => WellKnownEncodings.Utf16;
    protected override EncodingInfo EncodingWithBom => WellKnownEncodingInfos.Utf16LeBom;
    protected override EncodingInfo EncodingWithoutBom => WellKnownEncodingInfos.Utf16LeNoBom;

    public override string[]? RequiredEncodingAnalyzers { get; } = { "UTF-16" };
    
    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
}