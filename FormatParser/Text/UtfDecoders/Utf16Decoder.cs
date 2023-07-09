using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf16Decoder : IUtfDecoder
{
    private readonly TextChecker textChecker;
    private readonly CodepointConverter codepointConverter;
    private readonly TextParserSettings settings;

    public Utf16Decoder(TextChecker textChecker, CodepointConverter codepointConverter, TextParserSettings settings)
    {
        this.textChecker = textChecker;
        this.codepointConverter = codepointConverter;
        this.settings = settings;
    }

    public bool TryDecode(InMemoryDeserializer deserializer, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding)
    {
        if (TryParseInternal(deserializer, stringBuilder, Endianness.BigEndian))
        {
            encoding = WellKnownEncodings.UTF16BeNoBom;
            return true;
        }
        
        if (TryParseInternal(deserializer, stringBuilder, Endianness.LittleEndian))
        {
            encoding = WellKnownEncodings.UTF16LeNoBom;
            return true;
        }

        encoding = null;
        return false;
    }
    
    public string[] CanReadEncodings { get; } = { WellKnownEncodings.UTF16BeBom, WellKnownEncodings.UTF16LeBom, WellKnownEncodings.UTF16BeNoBom, WellKnownEncodings.UTF16LeNoBom  };

    private bool TryParseInternal(InMemoryDeserializer deserializer, StringBuilder stringBuilder, Endianness endianness)
    {
        deserializer.Offset = 0;
        deserializer.SetEndianess(endianness);
        
        stringBuilder.Clear();
        var processedChars = 0;
        
        while (deserializer.CanRead(sizeof(ushort)))
        {
            var codepoint = GetNextCodepoint(deserializer);
            
            if (!textChecker.IsValidCodepoint(codepoint))
                return false;
            
            if (processedChars < settings.SampleSize)
            {
                codepointConverter.Convert(codepoint, stringBuilder);
                processedChars++;
            }
        }

        return true;
    }

    private static uint GetNextCodepoint(InMemoryDeserializer deserializer)
    {
        var current = deserializer.ReadUShort();
        
        if (current >= 0xd800 && current < 0xdc00)
        {
            if (!deserializer.TryReadUShort(out var next))
                throw new BinaryReaderException("Unexpected end of utf16 string.");
            return ((current & (uint)0x3ff) << 10) + (next & (uint)0x3ff) + (uint)0x10000;
        }

        return current;
    }
}