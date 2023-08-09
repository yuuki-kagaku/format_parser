using FormatParser.Domain;
using FormatParser.Text.Helpers;

namespace FormatParser.Text.TextAnalyzers;

// Heuristics based on next assumptions:
// There is decent probability, that file with random bytes could be interpreted as valid UTF-16
// There are 65536 codepoints in Basic Multilingual Plane, most of this codepoint used by CJK characters: 27584 CJK Unified Ideographs, 11184 Hangul syllables
// But in real text more than 99.85% of the text consists of 1093 mostly used syllables for Korean language
// In modern Japanese and Chinese it estimated, that regular text usually contains only around 5000 most used CJK Unified Ideographs.
// Rarely used CJK Unified Ideographs and Hangul syllables according to our estimation contains around 30000 characters.
// If there are a lot of this characters in file, we can safely assume it is probably not a UTF-16 file
// At current iteration heuristics works with non-CJK text and with modern text in CJK languages (including texts with mixed languages).
// It is possible, that for some Chinese and Japanese text with high usage of irregular characters this heuristics required some tuning.

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

        if (nonBmpCharsFrequency + commonAsciiCharsFrequency > NonBmpAndCommonCharactersFrequencyThreshold)
            return DetectionProbability.Low;

        if (unusualCjkBmpCharsFrequency > UnusualCjkBmpCharsFrequencyThreshold)
            return DetectionProbability.No;
        
        return DetectionProbability.Low;
    }

    private static double NonBmpAndCommonCharactersFrequencyThreshold => 0.50;
    private static double UnusualCjkBmpCharsFrequencyThreshold => 0.10;
    
    private bool IsUnusualCJKCharacter(char c)
    {
        if (commonCjkBmpCharacters.Contains(c))
            return false;
        
        if (CJKBlockHelper.IsCJKIdeographOrHangulSyllableFromBmp(c))
            return true;
        
        return false;
    }

    public string[] AnalyzerIds { get; } = { "UTF-16" };
    
    private static readonly HashSet<char> CommonChars = CommonlyUsedCharacters.EnglishChars
        .Concat(CommonlyUsedCharacters.CommonSpecialCharacters)
        .Concat(CommonlyUsedCharacters.CommonPunctuation)            
        .ToHashSet();
}