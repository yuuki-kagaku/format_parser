namespace FormatParser;

public interface IBinaryFormatDecoder
{
    Task<IData?> TryDecodeAsync(Deserializer deserializer);
}