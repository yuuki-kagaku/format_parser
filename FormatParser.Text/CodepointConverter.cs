namespace FormatParser.Text;

public class CodepointConverter
{
    public void Convert(ulong codepoint, List<char> buffer)
    {
        if(codepoint < 0xD800 || (codepoint > 0xDFFF && codepoint < 0x10000))
        {
            buffer.Add((char)(ushort)(codepoint));
        }

        codepoint -= 0x010000;

        buffer.Add((char)(((0b11111111110000000000 & codepoint) >> 10) + 0xD800));
        buffer.Add((char)(((0b00000000001111111111 & codepoint) >> 00) + 0xDC00));
    }
}