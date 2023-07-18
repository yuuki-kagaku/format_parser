using FormatParser.Domain;

namespace FormatParser.Text.EncodingAnalyzers;

public interface ITextAnalyzer
{
    DetectionProbability AnalyzeProbability(TextSample text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding);
    
    string[] SupportedLanguages { get; }
}