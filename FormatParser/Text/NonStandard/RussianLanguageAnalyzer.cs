using System.Text.RegularExpressions;

namespace FormatParser.Text;

public class RussianLanguageAnalyzer : ILanguageAnalyzer
{
    private static readonly Regex pattern = new Regex(string.Join('|', MostUsedRussianWords.Words), RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public bool IsCorrectText(string str)
    {
       return pattern.IsMatch(str);
    }

    public string[] SupportedLanguages { get; } = {"ru"};

    public DetectionProbability DetectionProbabilityOnSuccessfulMatch { get; } = DetectionProbability.High;
}