namespace FormatParser.CLI;

public class FormatParserCliSettings
{
    public int DegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public string Directory { get; set; }
    
    public bool IsInteractiveMode { get; set; } = false;
}