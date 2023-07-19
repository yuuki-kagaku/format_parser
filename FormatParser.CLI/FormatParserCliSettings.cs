using FormatParser.Text;

namespace FormatParser.CLI;

public class FormatParserCliSettings
{
    public int DegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public string? Directory { get; set; }
    public int BufferSize { get; set; } = 16384;
    
    public bool FailOnUnauthorizedAccessException { get; set; } = false;
    public bool ShowElapsedTime { get; set; } = false;
    public bool FailOnIOException { get; set; } = true;

    public TextFileParsingSettings TextFileParsingSettings { get; set; } = new();
}