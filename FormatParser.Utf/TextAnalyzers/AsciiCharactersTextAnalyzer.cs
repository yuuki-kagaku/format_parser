using FormatParser.Text.EncodingAnalyzers;
using FormatParser.Text.Helpers;

namespace FormatParser.Text.TextAnalyzers;

public class AsciiCharactersTextAnalyzer : ITextAnalyzer
{
    public DetectionProbability AnalyzeProbability(TextSample text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        
        foreach (var c in text.GetChars())
        {
            if (c > (char)127)
                return DetectionProbability.No;
        }
        
        if (IsUtf8(encoding))
        {
            if (!encoding.ContainsBom)
                clarifiedEncoding = encoding with { Name = WellKnownEncodings.ASCII };
            return DetectionProbability.High;
        }
            
        return DetectionProbability.MediumLow;
    }

    private static bool IsUtf8(EncodingInfo encoding) => encoding.Name == WellKnownEncodings.UTF8;

    public string[] SupportedLanguages { get; } = {"ASCII"};
}