using FormatParser.Domain;

namespace FormatParser.Text;

public record EncodingInfo(string Name, Endianness Endianness, bool ContainsBom)
{
    public static readonly EncodingInfo ASCII = new(WellKnownEncodings.ASCII, Endianness.NotAllowed, false);
    public static readonly EncodingInfo Utf8BOM = new(WellKnownEncodings.UTF8, Endianness.NotAllowed, false);
    public static readonly EncodingInfo Utf8NoBOM = new(WellKnownEncodings.UTF8, Endianness.NotAllowed, false);
    
    public static readonly EncodingInfo UTF16LeNoBom = new(WellKnownEncodings.UTF16, Endianness.LittleEndian, false);
    public static readonly EncodingInfo UTF16LeBom = new(WellKnownEncodings.UTF16, Endianness.LittleEndian, true);
    public static readonly EncodingInfo UTF16BeNoBom = new(WellKnownEncodings.UTF16, Endianness.BigEndian, false);
    public static readonly EncodingInfo UTF16BeBom = new(WellKnownEncodings.UTF16, Endianness.BigEndian, true);
    
    public static readonly EncodingInfo UTF32LeNoBom = new(WellKnownEncodings.UTF32, Endianness.LittleEndian, false);
    public static readonly EncodingInfo UTF32LeBom = new(WellKnownEncodings.UTF32, Endianness.LittleEndian, true);
    public static readonly EncodingInfo UTF32BeNoBom = new(WellKnownEncodings.UTF32, Endianness.BigEndian, false);
    public static readonly EncodingInfo UTF32BeBom = new(WellKnownEncodings.UTF32, Endianness.BigEndian, true);
}