using FormatParser.Domain;
using FormatParser.Helpers;
using FormatParser.Helpers.BinaryReader;

namespace FormatParser.Archives;

public class ArchDetector : IBinaryFormatDetector
{
    private static readonly byte[] MagicNumbers = "!<arch>\n"u8.ToArray();

    public async Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader)
    {
        if (binaryReader.Length < MagicNumbers.Length)
            return null;

        var header = await binaryReader.ReadBytesAsync(MagicNumbers.Length);
        
        return SequentialCollectionComparer<byte>.Instance.Equals(header, MagicNumbers) ? new ArchFileFormat() : null;
    }
}