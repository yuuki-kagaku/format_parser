using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class Utf16BeDecoder : DecoderBase, ITextDecoder
{
    private readonly HashSet<char> invalidChars;

    public Utf16BeDecoder(TextFileParsingSettings settings)
    {
        var characterValidationSettings = new CharacterValidationSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
        invalidChars = InvalidCharactersHelper.GetForbiddenChars(characterValidationSettings).ToHashSet();
    }

    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding)Encoding.BigEndianUnicode.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    protected override IReadOnlySet<char> InvalidCharacters => invalidChars;

    protected override int MinimalSizeOfInput => 8;
    
    public override string EncodingName => WellKnownEncodings.Utf16;
    
    public override bool RequireTextBasedFormatMatch { get; } = false;

    public override bool SupportBom => true;
    protected override EncodingInfo EncodingWithBom => WellKnownEncodingInfos.Utf16BeBom;
    protected override EncodingInfo EncodingWithoutBom => WellKnownEncodingInfos.Utf16BeNoBom;

    public override string[]? RequiredEncodingAnalyzers { get; } = { "UTF-16" };

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
}