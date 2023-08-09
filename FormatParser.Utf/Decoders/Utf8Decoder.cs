using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class Utf8Decoder : DecoderBase, ITextDecoder
{
    private readonly CharacterValidatorSettings settings;

    public Utf8Decoder(TextFileParsingSettings settings)
    {
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, settings.AllowC1ControlsForUtf, false);
    }

    protected override Decoder GetDecoder(int inputSize)
    {
        var encoding = (Encoding) Encoding.UTF8.Clone();
        encoding.DecoderFallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    protected override IReadOnlySet<char> InvalidCharacters => InvalidCharactersHelper.GetForbiddenChars(settings).ToHashSet();

    protected override int MinimalSizeOfInput => 0;
    
    public override string EncodingName => WellKnownEncodings.Utf8;

    public override bool SupportBom => true;
    protected override EncodingInfo EncodingWithBom => WellKnownEncodingInfos.Utf8Bom;
    protected override EncodingInfo EncodingWithoutBom => WellKnownEncodingInfos.Utf8NoBom;

    public override string[]? RequiredEncodingAnalyzers { get; } = { "ASCII" };
    
    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.Low;
}