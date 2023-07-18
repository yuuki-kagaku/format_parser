namespace FormatParser.Domain;

public static class BitnessExtensions
{
    public static string ToPrettyString(this Bitness bitness)
    {
        return bitness switch
        {
            Bitness.Bitness16 => "16-bit",
            Bitness.Bitness32 => "32-bit",
            Bitness.Bitness64 => "64-bit",
            _ => throw new ArgumentOutOfRangeException(nameof(bitness), bitness, null)
        };
    }
}