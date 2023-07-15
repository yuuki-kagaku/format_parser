namespace FormatParser.Text;

public class CodepointChecker
{
    private readonly HashSet<uint> invalidCharacters;

    private CodepointChecker(IEnumerable<uint> invalidCharacters)
    {
        this.invalidCharacters = invalidCharacters.ToHashSet();
    }

    public static CodepointChecker IllegalC0Controls(CodepointCheckerSettings settings)
        => new(ControlCharacters.NonTextC0Controls.Except(GetCharactersToAllow(settings)).Concat(GetCharactersToForbid(settings)));

    public static CodepointChecker IllegalC0AndC1Controls(CodepointCheckerSettings settings)
        => new(ControlCharacters.NonTextC0Controls.Concat(ControlCharacters.C1Controls).Except(GetCharactersToAllow(settings)).Concat(GetCharactersToForbid(settings)));


    public bool IsValidCodepoint(uint codepoint) => !invalidCharacters.Contains(codepoint);
    
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