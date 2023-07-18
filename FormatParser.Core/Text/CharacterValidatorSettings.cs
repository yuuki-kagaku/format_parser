namespace FormatParser.Text;

public record CharacterValidatorSettings(bool AllowEscapeChar, bool AllowFormFeed, bool AllowC1Controls, bool AllowNoncharactersAtEndOfBmp)
{
}