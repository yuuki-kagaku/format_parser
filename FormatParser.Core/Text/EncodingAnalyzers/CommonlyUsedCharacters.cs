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

    public static readonly char[] RussianChars =
    {
        'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х',
        'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я', 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л',
        'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', 'Ё', 'ё'
    };

    public static readonly char[] RussianPunctuation =
    {
        '«', '»', '–', '—', '№'
    };

}