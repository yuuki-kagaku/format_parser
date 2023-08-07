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

    public override IEnumerable<char> GetInvalidCharacters => InvalidCharactersHelper.GetForbiddenChars(settings);

    protected override bool SupportBom => true;
    protected override EncodingInfo EncodingWithBom => WellKnownEncodingInfos.Utf32LeBom;
    public override EncodingInfo EncodingWithoutBom => WellKnownEncodingInfos.Utf32LeNoBom;

    public override string[]? RequiredEncodingAnalyzers => null;
    protected override int MinimalSizeOfInput => 8;

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.Low;
}