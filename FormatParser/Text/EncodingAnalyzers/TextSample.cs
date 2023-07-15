using System.Text;

namespace FormatParser.Text;

public class TextSample
{
    private readonly StringBuilder stringBuilder;
    private readonly Lazy<string> text;
    private readonly Lazy<string> textInLowerCase;

    public TextSample(StringBuilder stringBuilder)
    {
        this.stringBuilder = stringBuilder;
        text = new Lazy<string>(() => stringBuilder.ToString(), LazyThreadSafetyMode.None);
        textInLowerCase = new Lazy<string>(() => text.Value.ToLower(), LazyThreadSafetyMode.None);
    }

    public string Text => text.Value;

    public string TextInLowerCase => textInLowerCase.Value;

    public StringBuilder.ChunkEnumerator GetChunkEnumerator() => stringBuilder.GetChunks();
}