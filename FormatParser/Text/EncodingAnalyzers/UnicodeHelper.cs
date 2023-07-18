namespace FormatParser.Text.EncodingAnalyzers;

public static class UnicodeHelper
{
    public static bool IsCJKIdeographOrHangulSyllableFromBmp(char c)
    {
        // CJK Unified Ideographs Extension A
        if (0x3400 <= c && c <= 0x4DBF)
            return true;
        
        // CJK Unified Ideographs
        if (0x4E00 <= c && c <= 0x9FFF)
            return true;
        
        // Hangul Syllables
        if (0xAC00 <= c && c <= 0xD7AF)
            return true;
        
        return false;
    }

    public static char Bom = (char)0xFEFF;
}