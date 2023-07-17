using FormatParser.Text;

namespace FormatParser.CLI;

public class FormatParserCliSettings
{
    public int DegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public string Directory { get; set; }
    
    public bool DoNotFailOnUnauthorizedAccess { get; set; } = true;

    public TextFileParsingSettings TextFileParsingSettings { get; set; } = new();
}