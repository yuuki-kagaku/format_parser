using System.Diagnostics.CodeAnalysis;

namespace FormatParser.Text;

public interface IUtfDecoder
{
    bool TryDecode(InMemoryDeserializer deserializer, List<char> buffer, [NotNullWhen(true)] out string? encoding);

    string[] CanReadEncodings { get; }
}