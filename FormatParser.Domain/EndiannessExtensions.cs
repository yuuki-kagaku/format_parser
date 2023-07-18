namespace FormatParser.Domain;

public static class EndiannessExtensions
{
    public static string ToPrettyString(this Endianness endianness) => endianness switch
    {
        Endianness.NotAllowed => "",
        Endianness.LittleEndian => "LE",
        Endianness.BigEndian => "BE",
        _ => throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null)
    };
}