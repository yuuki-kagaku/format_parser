namespace FormatParser.Text;

public class CharacterValidator
{
    private readonly HashSet<char> invalidCharacters;

    public CharacterValidator(IEnumerable<char> invalidCharacters) => this.invalidCharacters = invalidCharacters.ToHashSet();

    public bool AllCharactersIsValid(IEnumerable<char> chars)
    {
        foreach (var c in chars)
        {
            if (!IsValidCharacter(c))
            {
                Console.WriteLine($"invalid : {c} | {((uint)c).ToString("x8")}");

                foreach (var c2 in invalidCharacters)
                {
                    Console.WriteLine($"c2 : {((uint)c2).ToString("x8")}");
                }
                return false;
            }
        }

        return true;
    }

    private bool IsValidCharacter(char c) => !invalidCharacters.Contains(c);
}