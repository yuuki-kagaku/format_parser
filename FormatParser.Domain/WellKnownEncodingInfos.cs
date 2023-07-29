namespace FormatParser.Domain;

public static class WellKnownEncodingInfos
{
    public static readonly EncodingInfo Ascii = new(WellKnownEncodings.Ascii, Endianness.NotAllowed, false);
    public static readonly EncodingInfo Utf8Bom = new(WellKnownEncodings.Utf8, Endianness.NotAllowed, true);
    public static readonly EncodingInfo Utf8NoBom = new(WellKnownEncodings.Utf8, Endianness.NotAllowed, false);
    
    public static readonly EncodingInfo Utf16LeNoBom = new(WellKnownEncodings.Utf16, Endianness.LittleEndian, false);
    public static readonly EncodingInfo Utf16LeBom = new(WellKnownEncodings.Utf16, Endianness.LittleEndian, true);
    public static readonly EncodingInfo Utf16BeNoBom = new(WellKnownEncodings.Utf16, Endianness.BigEndian, false);
    public static readonly EncodingInfo Utf16BeBom = new(WellKnownEncodings.Utf16, Endianness.BigEndian, true);
    
    public static readonly EncodingInfo Utf32LeNoBom = new(WellKnownEncodings.Utf32, Endianness.LittleEndian, false);
    public static readonly EncodingInfo Utf32LeBom = new(WellKnownEncodings.Utf32, Endianness.LittleEndian, true);
    public static readonly EncodingInfo Utf32BeNoBom = new(WellKnownEncodings.Utf32, Endianness.BigEndian, false);
    public static readonly EncodingInfo Utf32BeBom = new(WellKnownEncodings.Utf32, Endianness.BigEndian, true);
}