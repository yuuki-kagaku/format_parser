using System.Text;
using FormatParser.Domain;

namespace FormatParser.Text;

public class EBCDICDecoder : NonUtfDecoder
{
    private readonly CharacterValidatorSettings settings;
    public EBCDICDecoder(TextFileParsingSettings settings)  => 
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);

    protected override IReadOnlySet<char> InvalidCharacters => InvalidCharactersHelper
        .GetForbiddenChars(settings)
        .ToHashSet();
    
    public override string[]? RequiredEncodingAnalyzers { get; } = { "header_analyzer" };
    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
    protected override int MinimalSizeOfInput { get; } = 0;
    public override string EncodingName { get; } = "IBM037";

    protected override Decoder GetDecoder(int inputSize)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (Encoding)Encoding.GetEncoding("IBM037").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }
}