namespace FormatParser.Text;

public interface ITextAnalyzer
{
    DetectionProbability AnalyzeProbability(TextSample text, EncodingData encoding, out EncodingData? clarifiedEncoding);
    
    string[] SupportedLanguages { get; }
}