using FormatParser.Text.Helpers;

namespace FormatParser.Text;

public static class InvalidCharactersHelper
{
    public static IEnumerable<char> GetForbiddenChars(CharacterValidationSettings settings) =>
        ControlCharacters.NonTextC0Controls.Except(GetC0CharactersToAllow(settings))
            .Concat(GetForbiddenNoncharacters(settings))
            .Concat(GetForbiddenC1Controls(settings));

    private static IEnumerable<char> GetC0CharactersToAllow(CharacterValidationSettings settings)
    {
        if (settings.AllowEscapeChar)
            yield return ControlCharacters.EscapeCharCodepoint;
        
        if (settings.AllowFormFeed)
            yield return ControlCharacters.FormFeedCharCodepoint;
    }
    
    private static IEnumerable<char> GetForbiddenNoncharacters(CharacterValidationSettings settings)
    {
        if (settings.AllowNoncharactersAtEndOfBmp)
            yield break;

        yield return (char) 0xFFFE;
        yield return (char) 0xFFFF;
    }
    
    private static IEnumerable<char> GetForbiddenC1Controls(CharacterValidationSettings settings)
    {
        if (settings.AllowC1Controls)
            return Array.Empty<char>();

        return ControlCharacters.C1Controls;
    }
}