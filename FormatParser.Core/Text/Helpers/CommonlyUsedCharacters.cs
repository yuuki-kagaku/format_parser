namespace FormatParser.Text.EncodingAnalyzers;

public static class CommonlyUsedCharacters
{
    public static readonly char[] EnglishChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };
    
    public static readonly char[] CommonSpecialCharacters =
    {
        (char)0x0A, // CR
        (char)0x0D, // LF
        (char)0x09, // \t
        (char)0x20 // space
    };

    public static readonly char[] CommonPunctuation =
    {
        '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '@', ':', ';', '<', '=', '>', '?',
        '[', '\\', ']', '^', '_', '`'
    };

}