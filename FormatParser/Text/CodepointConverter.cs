using System.Text;

namespace FormatParser.Text;

public class CodepointConverter
{
    public void Convert(ulong codepoint, StringBuilder stringBuilder)
    {
        if (codepoint < 0xD800 || (codepoint > 0xDFFF && codepoint < 0x10000))
        {
            stringBuilder.Append((char)(ushort)(codepoint));
            return;
        }

        codepoint -= 0x010000;

        stringBuilder.Append((char)(((0xFFC00 & codepoint) >> 10) + 0xD800));
        stringBuilder.Append((char)(((0x3FF & codepoint) >> 00) + 0xDC00));
    }
}