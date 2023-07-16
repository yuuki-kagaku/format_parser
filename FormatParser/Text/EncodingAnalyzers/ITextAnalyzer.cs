namespace FormatParser.Text;

public interface ITextAnalyzer
{
    DetectionProbability AnalyzeProbability(TextSample text, string encoding, out string? clarifiedEncoding);
    
    TextAnalyzerType Type { get; }

    string[] SupportedLanguages { get; }
}