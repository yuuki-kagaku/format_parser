namespace FormatParser.Text;

public class CodepointChecker
{
    private static HashSet<uint> NonTextC0Controls =
        Enumerable.Range(0, 32)
            .Select(x => (uint) x)
            .Except(new uint[]
            {
               10, // CR
               13, // LF
               09, // \t
               27, // ESC
               0x0C,
            })
            .ToHashSet();
    
    public bool IsValidCodepoint(uint codepoint) => !NonTextC0Controls.Contains(codepoint);
}