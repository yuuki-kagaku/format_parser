namespace FormatParser.CLI;

public class FormatParserCliSettings
{
    public int DegreeOfParallelism { get; set; } = 1;
    public string Directory { get; set; }
    public bool IsInteractiveMode = true;
}