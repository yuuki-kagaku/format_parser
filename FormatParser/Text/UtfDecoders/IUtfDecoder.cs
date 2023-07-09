using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public interface IUtfDecoder
{
    bool TryDecode(InMemoryDeserializer deserializer, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding);

    string[] CanReadEncodings { get; }
}