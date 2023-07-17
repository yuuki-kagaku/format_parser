using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FormatParser.Test.Helpers;

namespace FormatParser.PerformanceTest;

[SimpleJob(RuntimeMoniker.Net70)]
public class Benchmark
{
    [GlobalSetup]
    public void Setup()
    {
    }

    [IterationSetup]
    public void ClearCache()
    {
        if (OperatingSystem.IsLinux())
            ShellRunner.RunCommand(@"sh", @"-c ""sync; echo 3 > /proc/sys/vm/drop_caches""");
    }

    [Benchmark]
    public void VarDir() => FormatParser.CLI.EntryPoint.Main(new []{"/var/"});

}