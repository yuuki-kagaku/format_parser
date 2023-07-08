namespace FormatParser.Text;

public class Utf8Parser
{
    private readonly TextChecker textChecker;
    private readonly bool crashAtSplitCharAtEnd;

    public Utf8Parser(TextChecker textChecker, bool crashAtSplitCharAtEnd)
    {
        this.textChecker = textChecker;
        this.crashAtSplitCharAtEnd = crashAtSplitCharAtEnd;
    }
    
    public bool TryParse(InMemoryDeserializer deserializer, out UtfEncoding encoding)
    {
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
                if (crashAtSplitCharAtEnd)
                    throw new Exception();
                else
                    return false;
                        
            result <<= 6;
            result |= (uint)b & 0x6F;
        }

        return true;
    }
}