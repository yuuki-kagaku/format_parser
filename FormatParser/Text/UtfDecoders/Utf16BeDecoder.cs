using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf16BeDecoder : Utf16Decoder
{
    public Utf16BeDecoder(CodepointConverter codepointConverter, TextParserSettings settings)
        : base(codepointConverter, settings)
    {
    }
    
    public override string[] CanReadEncodings { get; } = { WellKnownEncodings.UTF16BeBom, WellKnownEncodings.UTF16BeNoBom };

    public override bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)]  out string? encoding)
    {
        if (TryParseInternal(binaryReader, stringBuilder, Endianness.BigEndian, out var foundBom))
        {
            encoding = foundBom ? WellKnownEncodings.UTF16BeBom : WellKnownEncodings.UTF16BeNoBom;
            return true;
        }

        encoding = null;
        return false;
    }
}