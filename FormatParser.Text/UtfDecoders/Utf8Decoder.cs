namespace FormatParser.Text;

public class Utf8Decoder : IUtfDecoder
{
    private readonly TextChecker textChecker;
    private readonly CodepointConverter codepointConverter;
    private readonly TextParserSettings settings;

    public Utf8Decoder(TextChecker textChecker, CodepointConverter codepointConverter, TextParserSettings settings)
    {
        this.textChecker = textChecker;
        this.codepointConverter = codepointConverter;
        this.settings = settings;
    }
    
    public bool TryDecode(InMemoryDeserializer deserializer, List<char> buffer, out UtfEncoding encoding)
    {
        buffer.Clear();
        var processedChars = 0;
        try
        {
            deserializer.Offset = 0;
            while (TryGetNextCodepoint(deserializer, out var codepoint))
            {
                if (!textChecker.IsValidCodepoint(codepoint))
                {
                    encoding = UtfEncoding.Unknown;
                    return false;
                }

                if (processedChars < settings.SampleSize)
                {
                    codepointConverter.Convert(codepoint, buffer);
                    processedChars++;
                }
            }

            encoding = UtfEncoding.Utf8NoBOM;
            return true;
        }
        catch (Exception e)
        {
            encoding = UtfEncoding.Unknown;
            return false;
        }
    }

    private bool TryGetNextCodepoint(InMemoryDeserializer deserializer, out uint result)
    {
        result = 0;
        if (!deserializer.TryReadByte(out var b))
            return false;
        
        if (b < 0x80)
        {
            result = b;
            return true;
        }

        if (b < 0xC0)
        {
            throw new Exception();
        }
                    
        int size = 0;

        if (b < 0xE0)
        {
            result = (uint)b & 0x1F;
            size = 2;
        }
        else if (b < 0xF0)
        {
            result = (uint)b & 0x0F; 
            size = 3;
        }
        else if (b < 0xF8)
        {
            result = (uint)b & 0x07; 
            size = 4;
        }

        for(var i = 1; i < size; i++)
        {
            if  (!deserializer.TryReadByte(out b))
                if (settings.CrashAtSplitCharAtEnd)
                    throw new Exception();
                else
                    return false;
                        
            result <<= 6;
            result |= (uint)b & 0x6F;
        }

        return true;
    }
}