namespace FormatParser;

public interface IFormatDecoder
{
    Task<IData?> TryDecodeAsync(Deserializer deserializer);
}