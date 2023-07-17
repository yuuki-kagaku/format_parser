using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf16LeDecoder : Utf16Decoder
{
    public Utf16LeDecoder(CodepointConverter codepointConverter, TextFileParsingSettings settings) : base(codepointConverter, settings)
    {
    }

    public override string[] CanReadEncodings { get; } = { WellKnownEncodings.UTF16LeBom, WellKnownEncodings.UTF16LeNoBom };
    
    public override bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)]  out string? encoding)
    {
        if (TryParseInternal(binaryReader, stringBuilder, Endianness.LittleEndian, out var foundBom))
        {
            encoding = foundBom ? WellKnownEncodings.UTF16LeBom : WellKnownEncodings.UTF16LeNoBom;
            return true;
        }

        encoding = null;
        return false;
    }
}