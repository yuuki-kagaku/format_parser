namespace FormatParser.Text;

public class CodepointChecker
{
    private readonly HashSet<uint> invalidCharacters;

    public CodepointChecker(IEnumerable<uint> invalidCharacters)
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
    
    public bool IsValidCodepoint(uint codepoint) => !invalidCharacters.Contains(codepoint);
    
    public static IEnumerable<uint> IllegalC0Controls(CodepointCheckerSettings settings)
        => ControlCharacters.NonTextC0Controls.Except(GetCharactersToAllow(settings)).Concat(GetCharactersToForbid(settings));

    public static IEnumerable<uint> IllegalC0AndC1Controls(CodepointCheckerSettings settings)
        => ControlCharacters.NonTextC0Controls.Concat(ControlCharacters.C1Controls).Except(GetCharactersToAllow(settings)).Concat(GetCharactersToForbid(settings));

    
    private static IEnumerable<uint> GetCharactersToAllow(CodepointCheckerSettings settings)
    {
        if (settings.AllowEscapeChar)
            yield return ControlCharacters.EscapeCharCodepoint;
        
        if (settings.AllowFormFeed)
            yield return ControlCharacters.FormFeedCharCodepoint;
    }
    
    private static IEnumerable<uint> GetCharactersToForbid(CodepointCheckerSettings settings)
    {
        foreach (var invalidCodepoint in settings.AdditionalInvalidCodepoints)
            yield return invalidCodepoint;
    }
}