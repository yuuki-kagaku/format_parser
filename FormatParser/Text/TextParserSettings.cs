namespace FormatParser.Text;

public record TextParserSettings(int SampleSize, bool CrashAtSplitCharAtEnd, bool CrashIfUtf16InputIsNotEven)
{
    public static readonly TextParserSettings Default = new(4096, false, true);
}