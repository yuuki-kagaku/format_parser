using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf16BeDecoder : Utf16Decoder
{
    public Utf16BeDecoder(CodepointChecker codepointChecker, CodepointConverter codepointConverter, TextParserSettings settings)
        : base(codepointChecker, codepointConverter, settings)
    {
    }
    
    public override string[] CanReadEncodings { get; } = { WellKnownEncodings.UTF16BeBom, WellKnownEncodings.UTF16BeNoBom };

    public override bool TryDecode(InMemoryDeserializer deserializer, StringBuilder stringBuilder,[NotNullWhen(true)] out string? encoding, out DetectionProbability detectionProbability)
    {
        if (TryParseInternal(deserializer, stringBuilder, Endianness.BigEndian, out var foundBom, out detectionProbability))
        {
            encoding = foundBom ? WellKnownEncodings.UTF16BeBom : WellKnownEncodings.UTF16BeNoBom;
            return true;
        }

        encoding = null;
        return false;
    }
}