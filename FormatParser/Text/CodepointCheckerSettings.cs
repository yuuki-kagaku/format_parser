namespace FormatParser.Text;

public record CodepointCheckerSettings(bool AllowEscapeChar, bool AllowFormFeed, uint[] AdditionalInvalidCodepoints)
{
    public static readonly CodepointCheckerSettings Default = new (true, true, Array.Empty<uint>());
}