using FormatParser.Domain;

namespace FormatParser.Text;

public interface ITextAnalyzer
{
    DetectionProbability AnalyzeProbability(string text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding);
    
    string[] AnalyzerIds { get; }
}