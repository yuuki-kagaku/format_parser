namespace ConsoleApp1;

public class QualityCheckerSettings
{
    public string Utility { get; set; } = "file";
    public string UtilityArgs { get; set; } = "-bi";
    public bool PrintMismatchedFilesInfo { get; set; } = true;
    public int BufferSize { get; set; } = 4096;
}