namespace FormatParser.Text;

public interface ILanguageAnalyzer
{
    bool IsCorrectText(string text);

    string[] SupportedLanguages { get; }
    
    DetectionProbability DetectionProbabilityOnSuccessfulMatch { get; }
}