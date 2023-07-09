using System.Collections.Concurrent;

namespace FormatParser.CLI;

public class ForamatParserCliState
{
    public ConcurrentDictionary<IFileFormatInfo, AtomicInt> Occurence { get; } = new();
    public bool FileDiscoveryFinished = false;
    


}