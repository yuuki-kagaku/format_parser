namespace FormatParser.Text;

public record CodepointValidatorSettings(bool AllowEscapeChar, bool AllowFormFeed, bool AllowC1Controls, bool AllowNoncharactersAtEndOfBmp)
{
}