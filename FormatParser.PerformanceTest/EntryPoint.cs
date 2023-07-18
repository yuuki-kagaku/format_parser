using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace FormatParser.PerformanceTest;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        // BenchmarkWithDisk.Directory = args[0];
        // BenchmarkWithDisk.Directory = args[0];
        
        var summary = BenchmarkRunner.Run<BenchmarkWithDisk>();

        Console.WriteLine(summary);
    }
}