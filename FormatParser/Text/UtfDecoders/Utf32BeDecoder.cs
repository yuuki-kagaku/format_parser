using System.Text;
using FormatParser.Text.Encoding;

namespace FormatParser.Text;

public class Utf32BeDecoder : DecoderBase, IUtfDecoder
{
    protected override System.Text.Decoder GetDecoder(int inputSize)
    {
        var encoding = (System.Text.Encoding) new UTF32Encoding(true, true, true).Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ExceptionFallback;
        return encoding.GetDecoder();
    }

    public override HashSet<uint> GetInvalidCharacters { get; } = CodepointChecker
        .IllegalC0Controls(CodepointCheckerSettings.Default)
        .ToHashSet();
    
    public override int MinimalSizeOfInput { get; } = 8;

    public override bool SupportBom { get; } = true;
    public override string EncodingWithBom { get; } = WellKnownEncodings.UTF32BeBom;
    public override string EncodingWithoutBom { get; } = WellKnownEncodings.UTF32BeNoBom;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;
}