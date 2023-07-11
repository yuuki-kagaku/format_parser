namespace FormatParser.Text;

internal static class UTFConstants
{
    public static readonly (byte[], string)[] BOMs =
    {
        (new byte[]{0xFF, 0xFE, 0x00, 0x00}, WellKnownEncodings.UTF32LeBom),
        (new byte[]{0x00, 0x00, 0xFE, 0xFF}, WellKnownEncodings.UTF32LeBom),
        (new byte[]{0xEF, 0xBB, 0xBF}, WellKnownEncodings.Utf8BOM),
        (new byte[]{0xFE, 0xFF}, WellKnownEncodings.UTF16BeBom),
        (new byte[]{0xFF, 0xFE}, WellKnownEncodings.UTF16LeBom),
    };
}