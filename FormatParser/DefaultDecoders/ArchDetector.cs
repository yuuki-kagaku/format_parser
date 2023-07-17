using FormatParser.Helpers;

namespace FormatParser.DefaultDecoders;

public class ArchDetector : IBinaryFormatDetector
{
    private static byte[] MagicNumbers = "!<arch>\n"u8.ToArray();

    public async Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader)
    {
        if (binaryReader.Length < MagicNumbers.Length)
            return null;

        var header = await binaryReader.ReadBytesAsync(MagicNumbers.Length);
        
        return ArrayComparer<byte>.Instance.Equals(header, MagicNumbers) ? new ArchFileFormat() : null;
    }
}