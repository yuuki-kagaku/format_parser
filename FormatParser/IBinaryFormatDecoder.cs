namespace FormatParser;

public interface IBinaryFormatDecoder
{
    Task<IFileFormatInfo?> TryDecodeAsync(StreamingBinaryReader streamingBinaryReader);
}