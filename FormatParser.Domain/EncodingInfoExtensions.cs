namespace FormatParser.Domain;

public static class EncodingInfoExtensions
{
    public static string ToPrettyString(this EncodingInfo encodingInfo) => 
        $"{encodingInfo.Name}{EndiannessInfo(encodingInfo)}{BomInfo(encodingInfo)}";

    private static string BomInfo(EncodingInfo encodingInfo)
    {
        if (encodingInfo.Name is not (WellKnownEncodings.Utf32 or WellKnownEncodings.Utf16 or WellKnownEncodings.Utf8))
            return String.Empty;

        return encodingInfo.ContainsBom ? " BOM" : " No BOM";
    }
    
    private static string EndiannessInfo(EncodingInfo encodingInfo) => encodingInfo.Endianness == Endianness.NotAllowed 
        ? string.Empty 
        : $" {encodingInfo.Endianness.ToPrettyString()}";
}