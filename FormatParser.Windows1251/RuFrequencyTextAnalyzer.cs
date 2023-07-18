using FormatParser.Text;
using FormatParser.Text.EncodingAnalyzers;

namespace FormatParser.Windows1251;

public class RuFrequencyTextAnalyzer : ITextAnalyzer
{
    private static readonly HashSet<char> CommonCharacters = CommonlyUsedCharacters.EnglishChars
        .Concat(CommonlyUsedCharacters.CommonSpecialCharacters)
        .Concat(CommonlyUsedCharacters.CommonPunctuation)
        .Concat(CommonlyUsedCharacters.RussianChars)
        .Concat(CommonlyUsedCharacters.RussianPunctuation)
        .ToHashSet();

    public DetectionProbability AnalyzeProbability(TextSample text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        var totalChars = 0;
        var commonChars = 0;
        
        foreach (var c in text.GetChars())
        {
            totalChars++;
            if (CommonCharacters.Contains(c))
                commonChars++;
        }

        if (totalChars < MinimalCharsCount)
            return DetectionProbability.No;

        var frequency = (double)commonChars / (double)totalChars;

        if (frequency < Threshold)
            return DetectionProbability.No;

        return DetectionProbability.Medium;
    }

    private int MinimalCharsCount { get; } = 16;
    
    private double Threshold { get; } = 0.98;

    public string[] SupportedLanguages { get; } = new[] { "ru" };
}