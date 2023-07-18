namespace FormatParser.Text.EncodingAnalyzers;

public class UTF16Heuristics : IDefaultTextAnalyzer
{
    public DetectionProbability AnalyzeProbability(TextSample text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;

        var totalChars = 0;
        var nonBmpChars = 0;
        var unusualCjkBmpChars = 0;
        var commonAsciiChars= 0;

        var chars = text.GetChars();
        for (var i = 0; i < chars.Count; i++)
        {
            var c = chars[i];
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

    private static bool IsUnusualCJKCharacter(char c)
    {
        if (CommonCjkBmpCharacters.Contains(c))
            return false;
        
        if (UnicodeHelper.IsCJKIdeographOrHangulSyllableFromBmp(c))
            return true;
        
        return false;
    }

    public string[] SupportedLanguages { get; } = {"*"};
    
    private static readonly HashSet<char> CommonChars = CommonlyUsedCharacters.EnglishChars
        .Concat(CommonlyUsedCharacters.CommonSpecialCharacters)
        .Concat(CommonlyUsedCharacters.CommonPunctuation)            
        .ToHashSet();

    private static readonly HashSet<char> CommonCjkBmpCharacters =
        CommonCJKChars.MostCommonHangul
            .Concat(CommonCJKChars.CJKPunctuation)
            .Concat(CommonCJKChars.MostCommonChineseCharacters)
            .Concat(CommonCJKChars.Kana)
            .Concat(CommonCJKChars.MostUsedKanji)
            .Concat(CommonCJKChars.FullwidthAndHalfwidthForms)
            .ToHashSet();
}