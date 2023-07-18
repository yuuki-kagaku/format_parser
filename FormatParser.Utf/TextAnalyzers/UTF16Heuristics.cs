using FormatParser.Domain;
using FormatParser.Text.Helpers;

namespace FormatParser.Text.TextAnalyzers;

public class UTF16Heuristics : ITextAnalyzer
{
    private readonly HashSet<char> commonCjkBmpCharacters;

    public UTF16Heuristics() : this(new CommonCJKCharactersProvider()) {}
    public UTF16Heuristics(CommonCJKCharactersProvider commonCjkCharactersProvider)
    {
        commonCjkBmpCharacters = commonCjkCharactersProvider.MostUsedHangul
            .Concat(commonCjkCharactersProvider.MostUsedChineseCharacters)
            .Concat(commonCjkCharactersProvider.MostUsedKanji)
            .ToHashSet();
    }

    public DetectionProbability AnalyzeProbability(string text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;

        var totalChars = 0;
        var nonBmpChars = 0;
        var unusualCjkBmpChars = 0;
        var commonAsciiChars= 0;
        
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            totalChars++;

            if (Char.IsSurrogate(c))
            {
                nonBmpChars++;
                i++;
                continue;
            }

            if (CommonChars.Contains(c))
                commonAsciiChars++;

            if (IsUnusualCJKCharacter(c))
                unusualCjkBmpChars++;
        }

        var nonBmpCharsFrequency = (double)nonBmpChars / (double)totalChars;
        var unusualCjkBmpCharsFrequency = (double)unusualCjkBmpChars / (double)totalChars;
        var commonAsciiCharsFrequency = (double)commonAsciiChars / (double)totalChars;

        if (nonBmpCharsFrequency + commonAsciiCharsFrequency > 0.50)
            return DetectionProbability.Low;

        if (unusualCjkBmpCharsFrequency > 0.10)
            return DetectionProbability.No;
        
        return DetectionProbability.Low;
    }

    private bool IsUnusualCJKCharacter(char c)
    {
        if (commonCjkBmpCharacters.Contains(c))
            return false;
        
        if (CJKBlockHelper.IsCJKIdeographOrHangulSyllableFromBmp(c))
            return true;
        
        return false;
    }

    public string[] RequiredAnalyzers { get; } = {"UTF-16"};
    
    private static readonly HashSet<char> CommonChars = CommonlyUsedCharacters.EnglishChars
        .Concat(CommonlyUsedCharacters.CommonSpecialCharacters)
        .Concat(CommonlyUsedCharacters.CommonPunctuation)            
        .ToHashSet();
}