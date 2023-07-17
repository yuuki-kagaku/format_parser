using System.Text;
using FormatParser.Text.Encoding;

namespace FormatParser.Text;

public class Windows1251Decoder : DecoderBase
{
    public Windows1251Decoder(TextFileParsingSettings settings)
    {
        this.settings = settings;
    }
    
    private readonly TextFileParsingSettings settings;
    
    protected override Decoder GetDecoder(int inputSize) => Decoder;

    private static readonly Decoder Decoder = GetDecoder();

    public override HashSet<uint> GetInvalidCharacters { get; } = CodepointChecker
        .IllegalC0Controls(CodepointCheckerSettings.Default)
        .Concat(new uint[] { 152 })
        .ToHashSet();

    private static Decoder GetDecoder()
    {
        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = (System.Text.Encoding) System.Text.Encoding.GetEncoding("windows-1251").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }
    
    public override int MinimalSizeOfInput { get; } = 0;
    
    public override bool SupportBom { get; } = false;
    public override string EncodingWithBom => throw new NotSupportedException();
    public override string EncodingWithoutBom { get; } = "Windows-1251";

    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
    public override string? RequiredEncodingAnalyzer { get; } = "ru";
}