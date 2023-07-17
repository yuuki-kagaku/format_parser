namespace FormatParser.Text;

public class TextFileParsingSettings
{
    public int FileStreamBufferSize { get; set; } = 16384;
    public int SampleSize { get; set; } = 16384;
    public bool CrashAtSplitCharAtEnd { get; set; } = false;
    public bool CrashIfUtf16InputIsNotEven { get; set; } = true;
}