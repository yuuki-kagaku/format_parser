using FormatParser.Domain;

namespace FormatParser.Text.Helpers;

public static class EncodingInfoExtensions
{
    public static string ToPrettyString(this EncodingInfo encodingInfo, bool alwaysPrintBom) => 
        $"{Name(encodingInfo.Name)}{EndiannessInfo(encodingInfo)}{BomInfo(encodingInfo, alwaysPrintBom)}";

    private static string Name(string name) => 
        EncodingHelper.CanonicalEncodingNames.TryGetValue(name, out var normalizedName) ? normalizedName : name;

    private static string BomInfo(EncodingInfo encodingInfo, bool alwaysPrintBom)
    {
        if (!alwaysPrintBom && !EncodingHelper.SupportBom(encodingInfo.Name))
            return String.Empty;

        return encodingInfo.ContainsBom ? " BOM" : " No BOM";
    }
    
    private static string EndiannessInfo(EncodingInfo encodingInfo) => encodingInfo.Endianness == Endianness.NotAllowed 
        ? string.Empty 
        : $" {encodingInfo.Endianness.ToPrettyString()}";
}