namespace FormatParser.Text;

public class CodepointValidator
{
    private readonly HashSet<char> invalidCharacters;

    public CodepointValidator(IEnumerable<char> invalidCharacters)
    {
        this.invalidCharacters = invalidCharacters.ToHashSet();
    }

    public bool AllCharactersIsValid(ArraySegment<char> chars)
    {
        foreach (var c in chars)
        {
            if (!IsValidCodepoint(c))
                return false;
        }

        return true;
    }
    
    private bool IsValidCodepoint(char codepoint) => !invalidCharacters.Contains(codepoint);
    
    public static IEnumerable<char> GetForbiddenChars(CodepointValidatorSettings settings) =>
        ControlCharacters.NonTextC0Controls.Except(GetC0CharactersToAllow(settings))
            .Concat(GetForbiddenNoncharacters(settings))
            .Concat(GetForbidenC1Controls(settings));

    private static IEnumerable<char> GetForbidenC1Controls(CodepointValidatorSettings settings)
    {
        if (settings.AllowC1Controls)
            return Array.Empty<char>();

        return ControlCharacters.C1Controls;
    }
    
    private static IEnumerable<char> GetC0CharactersToAllow(CodepointValidatorSettings settings)
    {
        if (settings.AllowEscapeChar)
            yield return ControlCharacters.EscapeCharCodepoint;
        
        if (settings.AllowFormFeed)
            yield return ControlCharacters.FormFeedCharCodepoint;
    }
    
    private static IEnumerable<char> GetForbiddenNoncharacters(CodepointValidatorSettings settings)
    {
        if (settings.AllowNoncharactersAtEndOfBmp)
           yield break;

        yield return (char) 0xFFFE;
        yield return (char) 0xFFFF;
    }
}