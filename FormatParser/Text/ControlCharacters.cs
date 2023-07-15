namespace FormatParser.Text;

public static class ControlCharacters
{
    public static IReadOnlySet<uint> NonTextC0Controls =
        Enumerable.Range(0, 32)
            .Select(x => (uint) x)
            .Except(new uint[]
            {
                0x0A, // CR
                0x0D, // LF
                0x09, // \t
            })
            .Concat(new []{(uint)127}) // delete character, technically not a part of C0 controls
            .ToHashSet();
    
    public static IReadOnlySet<uint> C1Controls =
        Enumerable.Range(128, 32)
            .Select(x => (uint) x)
            .ToHashSet();

    public static uint EscapeCharCodepoint = 27;
    public static uint FormFeedCharCodepoint = 0x0C;
}