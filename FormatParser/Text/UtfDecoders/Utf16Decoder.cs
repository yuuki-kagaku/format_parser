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

    public bool TryDecode(InMemoryDeserializer deserializer, List<char> buffer, out Encoding encoding)
    {
        if (TryParseInternal(deserializer, buffer, Endianness.BigEndian))
        {
            encoding = Encoding.UTF16BeNoBom;
            return true;
        }
        
        if (TryParseInternal(deserializer, buffer, Endianness.LittleEndian))
        {
            encoding = Encoding.UTF16LeNoBom;
            return true;
        }

        encoding = Encoding.Unknown;
        return false;
    }

    public bool MatchEncoding(Encoding encoding) 
        => encoding is Encoding.UTF16BeBom or Encoding.UTF16LeBom or Encoding.UTF16BeNoBom or Encoding.UTF16LeNoBom;

    private bool TryParseInternal(InMemoryDeserializer deserializer, List<char> buffer, Endianness endianness)
    {
        deserializer.Offset = 0;
        deserializer.SetEndianess(endianness);
        
        buffer.Clear();
        var processedChars = 0;
        
        while (deserializer.CanRead(sizeof(ushort)))
        {
            var codepoint = GetNextCodepoint(deserializer);
            
            if (!textChecker.IsValidCodepoint(codepoint))
                return false;
            
            if (processedChars < settings.SampleSize)
            {
                codepointConverter.Convert(codepoint, buffer);
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
                throw new DeserializerException("Unexpected end of utf16 string.");
            return ((current & (uint)0x3ff) << 10) + (next & (uint)0x3ff) + (uint)0x10000;
        }

        return current;
    }
}