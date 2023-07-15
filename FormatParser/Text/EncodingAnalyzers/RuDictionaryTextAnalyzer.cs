using System.Text.RegularExpressions;

namespace FormatParser.Text;

public class RuDictionaryTextAnalyzer : ITextAnalyzer
{
    private static readonly Regex pattern = new (string.Join('|', MostUsedRussianWords.Words), RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public DetectionProbability AnalyzeProbability(TextSample text, string encoding, out string? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        return pattern.IsMatch(text.Text) ? DetectionProbability.High : DetectionProbability.No;
    }

    public TextAnalyzerType Type { get; } = TextAnalyzerType.Dictionary;

    public string[] SupportedLanguages { get; } = {"ru"};
}