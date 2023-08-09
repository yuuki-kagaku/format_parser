using System.Text.RegularExpressions;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Windows1251;

public class RuDictionaryTextAnalyzer : ITextAnalyzer
{
    private readonly Regex pattern;

    public RuDictionaryTextAnalyzer(RussianWordsProvider russianWordsProvider) => 
        pattern = new Regex(string.Join('|', russianWordsProvider.GetWords), RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public RuDictionaryTextAnalyzer() : this(new RussianWordsProvider()) { }

    public DetectionProbability AnalyzeProbability(string text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        return pattern.IsMatch(text.ToLower()) ? DetectionProbability.High : DetectionProbability.No;
    }

    public string[] AnalyzerIds { get; } = { "ru" };
}