namespace FormatParser.Text.Helpers;

public static class ControlCharacters
{
    public static IReadOnlySet<char> NonTextC0Controls =
        Enumerable.Range(0, 32)
            .Select(x => (char) x)
            .Except(new char[]
            {
                (char)0x0A, // CR
                (char)0x0D, // LF
                (char)0x09, // \t
            })
            .Concat(new []{(char)0x7F}) // delete character, technically not a part of C0 controls
            .ToHashSet();
    
    public static IReadOnlySet<char> C1Controls =
        Enumerable.Range(128, 32)
            .Select(x => (char) x)
            .ToHashSet();

    public static readonly char EscapeCharCodepoint = (char)27;
    public static readonly char FormFeedCharCodepoint = (char)0x0C;
}