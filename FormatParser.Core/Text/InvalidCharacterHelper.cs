using FormatParser.Text.Helpers;

namespace FormatParser.Text;

public static class InvalidCharacterHelper
{
    public static IEnumerable<char> GetForbiddenChars(CharacterValidatorSettings settings) =>
        ControlCharacters.NonTextC0Controls.Except(GetC0CharactersToAllow(settings))
            .Concat(GetForbiddenNoncharacters(settings))
            .Concat(GetForbidenC1Controls(settings));

    private static IEnumerable<char> GetForbidenC1Controls(CharacterValidatorSettings settings)
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