using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf16LeDecoder : Utf16Decoder
{
    public Utf16LeDecoder(CodepointChecker codepointChecker, CodepointConverter codepointConverter, TextParserSettings settings) : base(codepointChecker, codepointConverter, settings)
    {
    }

    public override string[] CanReadEncodings { get; } = { WellKnownEncodings.UTF16LeBom, WellKnownEncodings.UTF16LeNoBom };
    
    public override bool TryDecode(InMemoryDeserializer deserializer, StringBuilder stringBuilder,[NotNullWhen(true)] out string? encoding, out DetectionProbability detectionProbability)
    {
        if (TryParseInternal(deserializer, stringBuilder, Endianness.LittleEndian, out var foundBom, out detectionProbability))
        {
            encoding = foundBom ? WellKnownEncodings.UTF16LeBom : WellKnownEncodings.UTF16LeNoBom;
            return true;
        }

        encoding = null;
        return false;
    }
}