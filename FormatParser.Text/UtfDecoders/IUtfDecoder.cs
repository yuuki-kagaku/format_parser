namespace FormatParser.Text;

public interface IUtfDecoder
{
    bool TryDecode(InMemoryDeserializer deserializer, List<char> buffer, out Encoding encoding);
    bool MatchEncoding(Encoding encoding);
}