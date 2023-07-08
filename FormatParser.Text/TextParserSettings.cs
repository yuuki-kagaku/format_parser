namespace FormatParser.Text;

public record TextParserSettings(int SampleSize, bool CrashAtSplitCharAtEnd)
{
    public static readonly TextParserSettings Default = new(4096, false);
}