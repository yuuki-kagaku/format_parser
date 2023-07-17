using FormatParser.BinaryReader;

namespace FormatParser;

public interface IBinaryFormatDetector
{
    Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader);
}