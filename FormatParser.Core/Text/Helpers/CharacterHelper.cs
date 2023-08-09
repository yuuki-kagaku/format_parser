namespace FormatParser.Text.Helpers;

public static class CharacterHelper
{
    public const char Bom = (char)0xFEFF;
    public static bool IsAscii(char c) => c <= ControlCharacters.Delete;
}