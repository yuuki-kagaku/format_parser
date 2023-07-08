namespace FormatParser.Text;

public class Utf16Parser
{
    private readonly TextChecker textChecker;

    public Utf16Parser(TextChecker textChecker)
    {
        this.textChecker = textChecker;
    }

    public bool TryParse(InMemoryDeserializer deserializer, out UtfEncoding encoding)
    {
        if (TryParseInternal(deserializer, Endianess.BigEndian))
        {
            encoding = UtfEncoding.UTF16BeNoBom;
            return true;
        }
        
        if (TryParseInternal(deserializer, Endianess.LittleEndian))
        {
            encoding = UtfEncoding.UTF16LeNoBom;
            return true;
        }

        encoding = UtfEncoding.Unknown;
        return false;
    }
    
    private bool TryParseInternal(InMemoryDeserializer deserializer, Endianess endianess)
    {
        deserializer.Offset = 0;
        deserializer.SetEndianess(endianess);
        
        while (deserializer.CanRead(sizeof(ushort)))
        {
            if (!textChecker.IsValidCodepoint(GetNextCodepoint(deserializer)))
                return false;
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