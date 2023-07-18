using FormatParser.Domain;
using FormatParser.Text.Helpers;

namespace FormatParser.Text.TextAnalyzers;

public class AsciiCharactersTextAnalyzer : ITextAnalyzer
{
    public DetectionProbability AnalyzeProbability(string text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        
        foreach (var c in text)
        {
            if (c > (char)127)
                return DetectionProbability.No;
        }
        
        if (IsUtf8(encoding))
        {
            if (!encoding.ContainsBom)
                clarifiedEncoding = encoding with { Name = WellKnownEncodings.Ascii };
            return DetectionProbability.High;
        }
            
        return DetectionProbability.MediumLow;
    }

    private static bool IsUtf8(EncodingInfo encoding) => encoding.Name == WellKnownEncodings.Utf8;

    public string[] RequiredAnalyzers { get; } = {"ASCII"};
}