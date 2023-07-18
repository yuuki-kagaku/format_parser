using FormatParser.Text.Helpers;

namespace FormatParser.Text;

public static class InvalidCharacterHelper
{
    public static IEnumerable<char> GetForbiddenChars(CharacterValidatorSettings settings)
    {
        Console.WriteLine($"BINGO:: {((uint)ControlCharacters.LF).ToString("x8")}");
        
        foreach (var control in ControlCharacters.NonTextC0Controls)
        {
            Console.WriteLine($"invalid :  | {((uint)control).ToString("x8")}");
        }
        
        return ControlCharacters.NonTextC0Controls.Except(GetC0CharactersToAllow(settings))
            .Concat(GetForbiddenNoncharacters(settings))
            .Concat(GetForbiddenC1Controls(settings));
    }

    private static IEnumerable<char> GetForbiddenC1Controls(CharacterValidatorSettings settings)
    {
        if (settings.AllowC1Controls)
            return Array.Empty<char>();

        return ControlCharacters.C1Controls;
    }
    
    private static IEnumerable<char> GetC0CharactersToAllow(CharacterValidatorSettings settings)
    {
        if (settings.AllowEscapeChar)
            yield return ControlCharacters.EscapeCharCodepoint;
        
        if (settings.AllowFormFeed)
            yield return ControlCharacters.FormFeedCharCodepoint;
    }
    
    private static IEnumerable<char> GetForbiddenNoncharacters(CharacterValidatorSettings settings)
    {
        if (settings.AllowNoncharactersAtEndOfBmp)
            yield break;

        yield return (char) 0xFFFE;
        yield return (char) 0xFFFF;
    }
}