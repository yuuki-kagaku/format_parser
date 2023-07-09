namespace FormatParser;

public interface IBinaryFormatDecoder
{
    Task<IFileFormatInfo?> TryDecodeAsync(Deserializer deserializer);
}