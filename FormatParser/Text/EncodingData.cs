namespace FormatParser.Text;

public record EncodingData(string Name, Endianness Endianness, bool ContainsBom)
{
    public static readonly EncodingData ASCII = new(WellKnownEncodings.ASCII, Endianness.NotAllowed, false);
    public static readonly EncodingData Utf8BOM = new(WellKnownEncodings.UTF8, Endianness.NotAllowed, false);
    public static readonly EncodingData Utf8NoBOM = new(WellKnownEncodings.UTF8, Endianness.NotAllowed, false);
    
    public static readonly EncodingData  UTF16LeNoBom = new(WellKnownEncodings.UTF16, Endianness.LittleEndian, false);
    public static readonly EncodingData  UTF16LeBom = new(WellKnownEncodings.UTF16, Endianness.LittleEndian, true);
    public static readonly EncodingData  UTF16BeNoBom = new(WellKnownEncodings.UTF16, Endianness.BigEndian, false);
    public static readonly EncodingData UTF16BeBom = new(WellKnownEncodings.UTF16, Endianness.BigEndian, true);
    
    public static readonly EncodingData  UTF32LeNoBom = new(WellKnownEncodings.UTF32, Endianness.LittleEndian, false);
    public static readonly EncodingData  UTF32LeBom = new(WellKnownEncodings.UTF32, Endianness.LittleEndian, true);
    public static readonly EncodingData UTF32BeNoBom = new(WellKnownEncodings.UTF32, Endianness.BigEndian, false);
    public static readonly EncodingData UTF32BeBom = new(WellKnownEncodings.UTF32, Endianness.BigEndian, true);
}