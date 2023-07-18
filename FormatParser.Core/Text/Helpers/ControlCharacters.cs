namespace FormatParser.Text.Helpers;

public static class ControlCharacters
{
    public static readonly char EscapeCharCodepoint = (char)0x1B;
    public static readonly char FormFeedCharCodepoint = (char)0x0C;
    public static readonly char CR = (char)0x0D;
    public static readonly char LF = (char)0x0A;
    public static readonly char Tab = (char)0x09;
    public static readonly char Delete = (char)0x7F;
    
    public static readonly IReadOnlySet<char> NonTextC0Controls =
        Enumerable.Range(0, 32)
            .Select(x => (char)x)
            .Except(new char[]
            {
                CR,
                LF,
                Tab,
            })
            .Concat(new []{Delete}) // delete character, technically positions outside C0 controls
            .ToHashSet();
    public static readonly IReadOnlySet<char> C1Controls =
        Enumerable.Range(128, 32)
            .Select(x => (char)x)
            .ToHashSet();
}