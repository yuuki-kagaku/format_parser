namespace FormatParser.Text;

public record CharacterValidationSettings(bool AllowEscapeChar, bool AllowFormFeed, bool AllowC1Controls, bool AllowNoncharactersAtEndOfBmp);