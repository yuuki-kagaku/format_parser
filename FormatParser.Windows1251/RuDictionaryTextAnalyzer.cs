using System.Text.RegularExpressions;
using FormatParser.Text;
using FormatParser.Text.EncodingAnalyzers;

namespace FormatParser.Windows1251;

public class RuDictionaryTextAnalyzer : ITextAnalyzer
{
    private static readonly Regex Pattern = new (string.Join('|', MostUsedRussianWords.Words), RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public DetectionProbability AnalyzeProbability(TextSample text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        return Pattern.IsMatch(text.Text) ? DetectionProbability.High : DetectionProbability.No;
    }

    public string[] SupportedLanguages { get; } = {"ru"};
}