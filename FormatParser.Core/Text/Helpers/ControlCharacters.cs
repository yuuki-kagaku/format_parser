namespace FormatParser.Text.Helpers;

public static class ControlCharacters
{
    static ControlCharacters()
    {
        CR = (char)0x0D;
        LF = (char)0x0A;
        Tab = (char)0x09;
        Delete = (char)0x7F;
        EscapeCharCodepoint = (char)0x1B;
        FormFeedCharCodepoint = (char)0x0C;
        
        NonTextC0Controls =
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

        C1Controls =
            Enumerable.Range(128, 32)
                .Select(x => (char)x)
                .ToHashSet();
    }

    public static readonly IReadOnlySet<char> NonTextC0Controls;
    public static IReadOnlySet<char> C1Controls;

    public static readonly char EscapeCharCodepoint;
    public static readonly char FormFeedCharCodepoint;
    public static readonly char CR;
    public static readonly char LF;
    public static readonly char Tab;
    public static readonly char Delete;
}