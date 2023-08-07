using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class Utf32LeDecoder : DecoderBase, ITextDecoder
{
    private readonly CharacterValidatorSettings settings;

    public Utf32LeDecoder(TextFileParsingSettings settings)
    {
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }
    
    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) new UTF32Encoding(false, true, true).Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    protected override IReadOnlySet<char> InvalidCharacters => InvalidCharactersHelper.GetForbiddenChars(settings).ToHashSet();

    public override bool SupportBom => true;
    protected override EncodingInfo EncodingWithBom => WellKnownEncodingInfos.Utf32LeBom;
    protected override EncodingInfo EncodingWithoutBom => WellKnownEncodingInfos.Utf32LeNoBom;
    
    public override string EncodingName => WellKnownEncodings.Utf32;

    public override string[]? RequiredEncodingAnalyzers => null;
    protected override int MinimalSizeOfInput => 8;

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.Lowest;
}