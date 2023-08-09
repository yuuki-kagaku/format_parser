using FormatParser.Domain;
using FormatParser.Text;
using FormatParser.Text.Helpers;

namespace FormatParser.Windows1251;

public class RuFrequencyTextAnalyzer : IFrequencyTextAnalyzer
{
    public RuFrequencyTextAnalyzer() { }

    public DetectionProbability AnalyzeProbability(string text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        var russianCharsCount = 0;
        var russianCharsAfterBasicLatinCount = 0;
        var basicLatinAndPunctuationCount = 0;
        var mostFrequentLettersCount = 0;
        var totalCount = 0;

        var previousIsBasicLatin = false;
        
        foreach (var c in text)
        {
            totalCount++;
            
            if (MostFrequentLetters.Contains(c))
                mostFrequentLettersCount++;
            
            if (IsRussianLetter(c))
            {
                russianCharsCount++;

                if (previousIsBasicLatin)
                    russianCharsAfterBasicLatinCount++;
            }

            if (BasicLatinAndPunctuation.Contains(c))
                basicLatinAndPunctuationCount++;
            
            previousIsBasicLatin = BasicLatin.Contains(c);
        }

        if ((double)russianCharsAfterBasicLatinCount / (double)russianCharsCount > RussianCharsAfterBasicLatinCountThreshold)
            return DetectionProbability.No;
        
        if (russianCharsCount < 30)
            return DetectionProbability.No;

        if (((double)mostFrequentLettersCount / (double)russianCharsCount) < MostFrequentRussianLettersThreshold)
            return DetectionProbability.No;

        if ((double)(basicLatinAndPunctuationCount + russianCharsCount) / (double)totalCount > BasicLatinPunctuationAndRussianLettersThreshold)
            return DetectionProbability.Low;

        return DetectionProbability.No;
    }

    public string[] AnalyzerIds { get; } = {"ru"};

    private double RussianCharsAfterBasicLatinCountThreshold => 0.05; 
    private double MostFrequentRussianLettersThreshold => 0.30; 
    private double BasicLatinPunctuationAndRussianLettersThreshold => 0.98; 
    
    private static bool IsRussianLetter(char c)
    {
        if (c is >= 'а' and <= 'я')
            return true;
        
        if (c is >= 'А' and <= 'Я')
            return true;

        if (c is 'ё' or 'Ё')
            return true;

        return false;
    }

    private static readonly IReadOnlySet<char> BasicLatinAndPunctuation = new HashSet<char>
    {
        '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6',
        '7', '8', '9', ':', ';', '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
        'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b',
        'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
        'y', 'z', '{', '|', '}', '~', ControlCharacters.Tab, ControlCharacters.CR, ControlCharacters.LF, ' '
    };

    private static readonly IReadOnlySet<char> BasicLatin = new HashSet<char>
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };
        
    private static readonly IReadOnlySet<char> MostFrequentLetters = new HashSet<char>
    {
        'О', 'Е', 'А', 'И', 'Н', 'Т', 'С',  'о', 'е', 'а', 'и', 'н', 'т', 'с'
    };
}