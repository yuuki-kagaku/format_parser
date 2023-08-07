using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text;

public class EBCDICDecoder : NonUtfDecoder
{
    private readonly CharacterValidatorSettings settings;
    public EBCDICDecoder(TextFileParsingSettings settings)  => 
        this.settings = new CharacterValidatorSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);

    public override IEnumerable<char> GetInvalidCharacters => InvalidCharactersHelper
        .GetForbiddenChars(settings)
        .ToHashSet();
    
    public override string[]? RequiredEncodingAnalyzers { get; } = { "header_analyzer" };
    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
    protected override int MinimalSizeOfInput { get; } = 0;
    protected override EncodingInfo EncodingInfo { get; } = new ("IBM037", Endianness.NotAllowed, false);

    protected override Decoder GetDecoder(int inputSize)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (Encoding)Encoding.GetEncoding("IBM037").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }
}