namespace FormatParser.Domain;

public static class EncodingInfoExtensions
{
    public static string ToPrettyString(this EncodingInfo encodingInfo)
    {
        return $"{encodingInfo.Name}{EndiannessInfo(encodingInfo)}{BomInfo(encodingInfo)}";
    }

    private static string BomInfo(EncodingInfo encodingInfo)
    {
        if (encodingInfo.Name is not (WellKnownEncodings.UTF32 or WellKnownEncodings.UTF16 or WellKnownEncodings.UTF8))
            return String.Empty;

        return encodingInfo.ContainsBom ? " BOM" : " No BOM";
    }
    
    private static string EndiannessInfo(EncodingInfo encodingInfo)
    {
        if (encodingInfo.Endianness == Endianness.NotAllowed)
            return String.Empty;

        return $" {encodingInfo.Endianness.ToPrettyString()}";
    }
}