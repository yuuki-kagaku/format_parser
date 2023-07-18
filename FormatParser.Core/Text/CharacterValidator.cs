namespace FormatParser.Text;

public class CharacterValidator
{
    private readonly HashSet<char> invalidCharacters;

    public CharacterValidator(IEnumerable<char> invalidCharacters)
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