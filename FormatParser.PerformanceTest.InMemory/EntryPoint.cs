using BenchmarkDotNet.Running;

namespace FormatParser.PerformanceTest;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<InMemoryBenchmark>();

        Console.WriteLine(summary);
    }
}