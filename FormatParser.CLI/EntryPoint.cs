using System.Text.Json;
using FormatParser.ArgumentParser;
using FormatParser.Helpers;
using FormatParser.Text;
using FormatParser.Text.Decoders;

namespace FormatParser.CLI;

public static class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = ParseSettings(args);

        PluginLoadHelper.LoadPlugins();
        
        var state = new FormatParserCliState();
        
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            PrintState(state);
        };

        var runner = CreateRunner(settings, GetOverrideSettings());

        if (!Directory.Exists(settings.Directory))
            throw new FormatParserException($"Directory {settings.Directory} does not exists.");
        
        runner.Run(settings, state).GetAwaiter().GetResult();

        if (settings.ShowElapsedTime)
        {
            Console.WriteLine($"Run complete. Elapsed: {state.Stopwatch!.Elapsed.TotalMilliseconds}");
            Console.WriteLine();
        }

        PrintState(state);
    }

    private static void PrintState(FormatParserCliState state)
    {
        foreach (var (formatInfo, occurence) in state.Occurence.OrderByDescending(x => x.Value.Read()))
        {
            Console.WriteLine($"[{occurence.Read().ToString(),6}] : {formatInfo.ToPrettyString()}");
        }
    }
    
    private static FormatParserCliSettings ParseSettings(string[] args)
    {
        var argsParser = new ArgumentParser<FormatParserCliSettings>()
            .WithPositionalArgument((settings, arg) => settings.Directory = arg)
            .OnNamedParameter("parallel", (settings, arg) => settings.DegreeOfParallelism = arg, false)
            .OnNamedParameter("show-elapsed", settings => settings.ShowElapsedTime = true, false)
            .OnNamedParameter("allow-frequency-analyzers", settings => settings.AllowFrequencyAnalyzers = true, false)
            .OnNamedParameter("buffer-size", (settings, arg) => settings.BufferSize = arg, false);

        return argsParser.Parse(args);
    }

    private static OverrideSettings GetOverrideSettings()
    {
        const string filename = "override.settings.json";
        if (!File.Exists(filename))
            return new();

        return JsonSerializer.Deserialize<OverrideSettings>(File.ReadAllText(filename)) ?? new ();
    }
    
    private static CLIRunner CreateRunner(FormatParserCliSettings settings, OverrideSettings overrideSettings)
    {
        var binaryFormatDetectors = ClassDiscoveryHelper.GetAllInstancesOf<IBinaryFormatDetector>().ToArray();

        var textAnalyzers = ClassDiscoveryHelper.GetAllInstancesOf<ITextAnalyzer>()
            .Where(x => !overrideSettings.DisabledAnalyzers.Any(a => string.Equals(a, x.GetType().FullName, StringComparison.InvariantCultureIgnoreCase)))
            .ToArray();

        if (!settings.AllowFrequencyAnalyzers)
            textAnalyzers = textAnalyzers.Where(x => x is not IFrequencyTextAnalyzer).ToArray();
        
        var textFileParsingSettings = new TextFileParsingSettings
        {
            SampleSize = settings.BufferSize,
        };

        var fileDiscovererSettings = new FileDiscovererSettings
        {
            FallOnUnauthorizedException = settings.FailOnUnauthorizedAccessException,
            FailOnIOException = settings.FailOnIOException
        };
        
        var textDecoders = ClassDiscoveryHelper.GetAllInstancesOf<ITextDecoder, TextFileParsingSettings>(textFileParsingSettings).ToArray();

        var textBasedFormatDetectors = ClassDiscoveryHelper.GetAllInstancesOf<ITextBasedFormatDetector>().ToArray();

        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            textDecoders,
            textAnalyzers);

        var streamFactory = new StreamFactory(settings.BufferSize);
        var formatDecoder = new FormatDetector(binaryFormatDetectors,
            textBasedFormatDetectors,
            compositeTextFormatDecoder,
            settings.TextFileParsingSettings
        );
        
        return new CLIRunner(
            new FileDiscoverer(fileDiscovererSettings), 
            formatDecoder,
            streamFactory,
            fileDiscovererSettings
        );
    }
}