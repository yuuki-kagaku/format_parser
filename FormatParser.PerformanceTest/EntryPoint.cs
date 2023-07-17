using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace FormatParser.PerformanceTest;

public class EntryPoint
{
  
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchmark>();

        Console.WriteLine(summary);
    }
}