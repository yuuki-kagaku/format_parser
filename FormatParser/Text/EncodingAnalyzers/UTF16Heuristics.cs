namespace FormatParser.Text;

public class UTF16Heuristics : IDefaultTextAnalyzer
{
    public DetectionProbability AnalyzeProbability(TextSample text, string encoding, out string? clarifiedEncoding)
    {
        clarifiedEncoding = null;

        var totalChars = 0;
        var nonBmpChars = 0;
        var unusualCjkBmpChars = 0;
        var commonAsciiChars= 0;

        foreach (var chunk in text.GetChunkEnumerator())
        {
            var span = chunk.Span;
            for (var i = 0; i < span.Length; i++)
            {
                var c = span[i];
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

                if (HighlyUnusualCharacters.Contains(c))
                    return DetectionProbability.No;
            }
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

    public TextAnalyzerType Type { get; } = TextAnalyzerType.Frequency;
    public string[] SupportedLanguages { get; } = {"*"};
    
    private static readonly HashSet<char> CommonChars = CommonlyUsedCharacters.EnglishChars
        .Concat(CommonlyUsedCharacters.CommonSpecialCharacters)
        .Concat(CommonlyUsedCharacters.CommonPunctuation)            
        .ToHashSet();

    private static readonly HashSet<char> CJKPunctiationCharacters =
        CommonCJKChars.CommonCJKPunctuation
            .ToHashSet();
    
    private static readonly HashSet<char> HighlyUnusualCharacters =
        new char[]
            {
                (char)0xFFFF
            }
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