namespace FormatParser.Text.EncodingAnalyzers;

public class AsciiCharactersTextAnalyzer : IDefaultTextAnalyzer
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
            clarifiedEncoding = encoding with { Name = WellKnownEncodings.ASCII };
            return DetectionProbability.High;
        }
            
        return DetectionProbability.MediumLow;
    }

    private static bool IsUtf8(EncodingInfo encoding) => encoding.Name == WellKnownEncodings.UTF8;

    public string[] SupportedLanguages { get; } = {"*"};
}