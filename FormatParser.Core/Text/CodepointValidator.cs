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
}