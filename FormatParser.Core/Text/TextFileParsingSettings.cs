namespace FormatParser.Text;

public class TextFileParsingSettings
{
    public int SampleSize { get; set; } = 16384;
    
    public bool CrashAtSplitCharAtEnd { get; set; } = false;
    
    public bool CrashIfUtf16InputIsNotEven { get; set; } = true;

    public bool AllowC1ControlsForUtf { get; set; } = true;
    
    public bool AllowEscapeChar { get; set; } = true; 
    
    public bool AllowFormFeed { get; set; } = true;
}