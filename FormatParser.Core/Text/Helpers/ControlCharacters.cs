using FormatParser.Helpers;

namespace FormatParser.Text.Helpers;

public static class ControlCharacters
{
    public const char EscapeCharCodepoint = (char)0x1B;
    public const char FormFeedCharCodepoint = (char)0x0C;
    public const char CR = (char)0x0D;
    public const char LF = (char)0x0A;
    public const char NEL = (char)0x85;
    public const char Tab = (char)0x09;
    public const char Delete = (char)0x7F;
    
    public static readonly IReadOnlySet<char> NonTextC0Controls =
        Enumerable.Range(0, 32)
            .Select(x => (char)x)
            .Except(new []
            {
                CR,
                LF,
                Tab,
            })
            .Concat(Delete) // delete character, technically positions outside C0 controls
            .ToHashSet();
    
    public static readonly IReadOnlySet<char> C1Controls =
        Enumerable.Range(128, 32)
            .Select(x => (char)x)
            .ToHashSet();

    public static bool IsNonTextC0Control(char c) => NonTextC0Controls.Contains(c);
}