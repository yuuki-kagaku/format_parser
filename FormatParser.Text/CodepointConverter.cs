using System.Text;

namespace FormatParser.Text;

public class CodepointConverter
{
    public string Convert(ulong codepoint)
    {
        var buffer = new List<char>();
        ProcessCharacter(codepoint);

        return new StringBuilder(buffer.Capacity).Append(buffer.ToArray()).ToString();

        void ProcessCharacter(ulong codepoint)
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
}