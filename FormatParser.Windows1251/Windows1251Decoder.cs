using System.Text;
using FormatParser.Domain;
using FormatParser.Helpers;
using FormatParser.Text;

namespace FormatParser.Windows1251;

public class Windows1251Decoder : NonUtfDecoder
{
    private readonly HashSet<char> invalidChars;

    public Windows1251Decoder(TextFileParsingSettings settings)
    {
        var characterValidationSettings = new CharacterValidationSettings(settings.AllowEscapeChar, settings.AllowFormFeed, true, false);
        invalidChars = InvalidCharactersHelper.GetForbiddenChars(characterValidationSettings).Concat((char)152).ToHashSet();
    }

    protected override IReadOnlySet<char> InvalidCharacters => invalidChars;

    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
    public override string[]? RequiredEncodingAnalyzers { get; } = { "ru" };
    
    public override bool RequireTextBasedFormatMatch { get; } = false;
    
    public override string EncodingName { get; } = "Windows-1251";

    protected override Decoder GetDecoder(int inputSize) => GetDecoder();

    protected override int MinimalSizeOfInput => 0;

    private static Decoder GetDecoder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (Encoding)Encoding.GetEncoding("windows-1251").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }
}