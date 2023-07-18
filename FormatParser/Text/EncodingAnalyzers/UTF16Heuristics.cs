namespace FormatParser.Text.EncodingAnalyzers;

public class UTF16Heuristics : IDefaultTextAnalyzer
{
    private readonly Lazy<HashSet<char>> commonCjkBmpCharacters;

    public UTF16Heuristics() : this(new CommonCJKCharactersProvider()) {}
    public UTF16Heuristics(CommonCJKCharactersProvider commonCjkCharactersProvider)
    {
        commonCjkBmpCharacters = new Lazy<HashSet<char>>(() => commonCjkCharactersProvider.MostUsedHangul
            .Concat(commonCjkCharactersProvider.MostUsedChineseCharacters)
            .Concat(commonCjkCharactersProvider.MostUsedKanji)
            .ToHashSet(), LazyThreadSafetyMode.PublicationOnly);
    }

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

    private bool IsUnusualCJKCharacter(char c)
    {
        if (commonCjkBmpCharacters.Value.Contains(c))
            return false;
        
        if (UnicodeHelper.IsCJKIdeographOrHangulSyllableFromBmp(c))
            return true;
        
        return false;
    }

    public string[] SupportedLanguages { get; } = {"UTF-16"};
    
    private static readonly HashSet<char> CommonChars = CommonlyUsedCharacters.EnglishChars
        .Concat(CommonlyUsedCharacters.CommonSpecialCharacters)
        .Concat(CommonlyUsedCharacters.CommonPunctuation)            
        .ToHashSet();

}