using System.Text;
using FormatParser.Helpers;

namespace FormatParser.Text;

public class TextSample
{
    private readonly Lazy<string> text;
    private readonly ArraySegment<char> chars;

    public TextSample(ArraySegment<char> chars)
    {
        this.chars = chars;
        text = new Lazy<string>(() =>
        {
            var stringBuilder = new StringBuilder(chars.Count);
            stringBuilder.Append(chars.ToMemory());
            
            return stringBuilder.ToString();
        }, LazyThreadSafetyMode.None);
    }

    public string Text => text.Value;
    
    public ArraySegment<char> GetChars() => chars;
}