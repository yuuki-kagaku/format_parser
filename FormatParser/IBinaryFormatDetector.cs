namespace FormatParser;

public interface IBinaryFormatDetector
{
    Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader);
}