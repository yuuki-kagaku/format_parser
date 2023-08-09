using System.Text;
using FormatParser.Text.Helpers;

namespace FormatParser.Text.Decoders;

/// <summary>
/// Decoder for that allows every C0 and C1 control symbols, except \0 and replaces them with ?.
/// Used for some legacy code pages, like DOS codepage 437.
/// Also this decoder supports BOM, because sometimes files incorrectly mix UTF-8 and Windows-1252.
/// </summary>
public class AsciiBasedEncodingDecoder : Decoder
{
    private readonly Decoder decoder;

    public AsciiBasedEncodingDecoder()
    {
        var encoding = (Encoding)Encoding.GetEncoding("UTF-8").Clone();
        var decoder = encoding.GetDecoder();
        decoder.Fallback = DecoderFallback.ReplacementFallback;
        this.decoder = decoder;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        return decoder.GetCharCount(bytes, index, count);
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        var result = decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        for (var i = 0; i < result; i++)
        {
            if (CharacterHelper.IsAscii(chars[i]) && !ControlCharacters.IsNonTextC0Control(chars[i])) 
                continue;
            
            if (i == 0 && chars[i] == CharacterHelper.Bom)
                continue;
                
            chars[i] = '?';
        }

        return result;
    }
}