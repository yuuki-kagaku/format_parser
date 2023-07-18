using FormatParser.Text;

namespace FormatParser.QualityChecker;

public class QualityCheckerSettings
{
    public string? Directory { get; set; }
    public string Utility { get; set; } = "file";
    public string UtilityArgs { get; set; } = "-bi";
    public bool PrintMismatchedFilesInfo { get; set; } = true;
    public int BufferSize { get; set; } = 4096;
    
    public TextFileParsingSettings TextFileParsingSettings { get; set; } = new();
}