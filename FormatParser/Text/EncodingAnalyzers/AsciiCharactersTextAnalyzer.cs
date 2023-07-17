namespace FormatParser.Text;

public class AsciiCharactersTextAnalyzer : IDefaultTextAnalyzer
{
    public DetectionProbability AnalyzeProbability(TextSample text, string encoding, out string? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        
        foreach (var c in text.GetChars())
        {
            if (c > (char)127)
                return DetectionProbability.No;
        }
        
        if (IsUtf8(encoding))
        {
            clarifiedEncoding = WellKnownEncodings.ASCII;
            return DetectionProbability.High;
        }
            
        return DetectionProbability.MediumLow;
    }

    private bool IsUtf8(string encoding) => encoding is WellKnownEncodings.Utf8BOM or WellKnownEncodings.Utf8NoBOM;

    public string[] SupportedLanguages { get; } = {"*"};
}