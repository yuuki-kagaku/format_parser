namespace FormatParser.Text;

public class CharacterValidator
{
    private readonly HashSet<char> invalidCharacters;

    public CharacterValidator(IEnumerable<char> invalidCharacters) => this.invalidCharacters = invalidCharacters.ToHashSet();

    public bool AllCharactersIsValid(IEnumerable<char> chars) => chars.All(IsValidCharacter);

    private bool IsValidCharacter(char c) => !invalidCharacters.Contains(c);
}