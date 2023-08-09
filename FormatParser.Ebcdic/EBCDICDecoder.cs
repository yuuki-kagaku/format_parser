using System.Text;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Ebcdic;

public class EBCDICDecoder : NonUtfDecoder
{
    private readonly HashSet<char> invalidChars;
    public EBCDICDecoder(TextFileParsingSettings settings)
    {
        var characterValidationSettings = new CharacterValidationSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);
        invalidChars = InvalidCharactersHelper.GetForbiddenChars(characterValidationSettings).ToHashSet();
    }

    protected override IReadOnlySet<char> InvalidCharacters => invalidChars;
    
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