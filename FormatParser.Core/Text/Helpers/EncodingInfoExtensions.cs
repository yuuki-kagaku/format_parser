using FormatParser.Domain;

namespace FormatParser.Text.Helpers;

public static class EncodingInfoExtensions
{
    public static string ToPrettyString(this EncodingInfo encodingInfo) => 
        $"{encodingInfo.Name}{EndiannessInfo(encodingInfo)}{BomInfo(encodingInfo)}";

    private static string BomInfo(EncodingInfo encodingInfo)
    {
        if (!EncodingHelper.SupportBom(encodingInfo.Name))
            return String.Empty;

        return encodingInfo.ContainsBom ? " BOM" : " No BOM";
    }
    
    private static string EndiannessInfo(EncodingInfo encodingInfo) => encodingInfo.Endianness == Endianness.NotAllowed 
        ? string.Empty 
        : $" {encodingInfo.Endianness.ToPrettyString()}";
}