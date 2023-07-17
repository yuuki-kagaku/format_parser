using System.Text.RegularExpressions;
using FormatParser.Text;

namespace FormatParser.Windows1251;

public class RuDictionaryTextAnalyzer : ITextAnalyzer
{
    private static readonly Regex pattern = new (string.Join('|', MostUsedRussianWords.Words), RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public DetectionProbability AnalyzeProbability(TextSample text, EncodingData encoding, out EncodingData? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        return pattern.IsMatch(text.Text) ? DetectionProbability.High : DetectionProbability.No;
    }

    public string[] SupportedLanguages { get; } = {"ru"};
}