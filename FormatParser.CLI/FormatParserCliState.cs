using System.Collections.Concurrent;
using System.Diagnostics;

namespace FormatParser.CLI;

public class FormatParserCliState
{
    public Stopwatch? Stopwatch { get; set; }
    public ConcurrentDictionary<IFileFormatInfo, AtomicInt> Occurence { get; } = new();
}