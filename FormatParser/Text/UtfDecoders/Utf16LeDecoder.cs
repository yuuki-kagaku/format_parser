using System.Diagnostics.CodeAnalysis;
using System.Text;
using FormatParser.Text.Encoding;

namespace FormatParser.Text;

public class Utf16LeDecoder : DecoderBase, IUtfDecoder
{
    protected override System.Text.Decoder GetDecoder(int inputSize)
    {
        var encoding = (System.Text.Encoding) System.Text.Encoding.Unicode.Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = FormatParserDecoderFallback.DoNotFailAtEndOfInput(inputSize, 4);
        return encoding.GetDecoder();
    }

    public override HashSet<uint> GetInvalidCharacters { get; } = CodepointChecker
        .IllegalC0Controls(CodepointCheckerSettings.Default)
        .ToHashSet();
    
    public override int MinimalSizeOfInput { get; } = 8;
    
    public override bool SupportBom { get; } = true;
    public override string EncodingWithBom { get; } = WellKnownEncodings.UTF16LeBom;
    public override string EncodingWithoutBom { get; } = WellKnownEncodings.UTF16LeNoBom;

    public override string? RequiredEncodingAnalyzer { get; } = null;
    
    public override DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.No;
}