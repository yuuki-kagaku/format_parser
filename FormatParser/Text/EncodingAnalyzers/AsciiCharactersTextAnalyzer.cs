namespace FormatParser.Text;

public class AsciiCharactersTextAnalyzer : IDefaultTextAnalyzer
{
    public DetectionProbability AnalyzeProbability(TextSample text, string encoding, out string? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        foreach (var chunk in text.GetChunkEnumerator())
        {
            var span = chunk.Span;

            foreach (var c in span)
            {
                if (c > (char)127)
                    return DetectionProbability.No;
            }
        }
        
        if (IsUtf8(encoding))
        {
            clarifiedEncoding = WellKnownEncodings.ASCII;
            return DetectionProbability.High;
        }
            
        return DetectionProbability.MediumLow;
    }

    private bool IsUtf8(string encoding) => encoding is WellKnownEncodings.Utf8BOM or WellKnownEncodings.Utf8NoBOM;

    public TextAnalyzerType Type { get; } = TextAnalyzerType.Frequency;
    public string[] SupportedLanguages { get; } = {"*"};
}