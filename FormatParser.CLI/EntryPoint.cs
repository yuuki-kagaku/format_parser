using System.Reflection;
using System.Text.Json;
using FormatParser.ArgumentParser;
using FormatParser.Text;
using FormatParser.Text.Decoders;

namespace FormatParser.CLI;

public static class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = ParseSettings(args);

        LoadPlugins();
        
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
        var binaryFormatDetectors = GetAllInstancesOf<IBinaryFormatDetector>().ToArray();

        var textAnalyzers = GetAllInstancesOf<ITextAnalyzer>()
            .Where(x => !overrideSettings.DisabledAnalyzers.Any(a => string.Equals(a, x.GetType().FullName, StringComparison.InvariantCultureIgnoreCase)))
            .ToArray();
        
        var textFileParsingSettings = new TextFileParsingSettings
        {
            SampleSize = settings.BufferSize,
        };

        var fileDiscovererSettings = new FileDiscovererSettings
        {
            FallOnUnauthorizedException = settings.FailOnUnauthorizedAccessException,
            FailOnIOException = settings.FailOnIOException
        };
        
        var textDecoders = GetAllInstancesOf<ITextDecoder, TextFileParsingSettings>(textFileParsingSettings).ToArray();

        var textBasedFormatDetectors = GetAllInstancesOf<ITextBasedFormatDetector>().ToArray();

        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            textDecoders, 
            textAnalyzers);

        var streamFactory = new StreamFactory(settings.BufferSize);
        var formatDecoder = new FormatDetector(binaryFormatDetectors,
            new TextFileProcessor(textBasedFormatDetectors, compositeTextFormatDecoder),
            settings.TextFileParsingSettings
        );
        
        return new CLIRunner(
            new FileDiscoverer(fileDiscovererSettings), 
            formatDecoder,
            streamFactory,
            fileDiscovererSettings
        );
    }

    private static void LoadPlugins()
    {
        var pluginDirectory = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}";

        if (!Directory.Exists(pluginDirectory))
            return;

        foreach (var directory in Directory.GetDirectories(pluginDirectory))
        {
            var plugins = Directory
                .EnumerateFiles(directory)
                .Where(x => x.EndsWith(".dll"));

            foreach (var dll in plugins)
            {
                try
                {
                    var a = Assembly.LoadFrom(dll);

                    if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName == a.FullName))
                        continue;

                    AppDomain.CurrentDomain.Load(a.GetName());
                }
                catch (FileLoadException)
                {
                }
                catch (BadImageFormatException)
                {
                }
            }
        }
    }

    private static IEnumerable<T> GetAllInstancesOf<T>()
    {
        var types = GetAllTypes<T>();

        foreach (var t in types.OrderBy(t => t.Name))
            yield return (T)Activator.CreateInstance(t)!;
    }
    
    private static IEnumerable<T> GetAllInstancesOf<T, TParam>(TParam param)
    {
        var types = GetAllTypes<T>();
        foreach (var t in types.OrderBy(t => t.Name))
            yield return (T)Activator.CreateInstance(t, param)!;
    }

    private static IEnumerable<Type> GetAllTypes<T>()
    {
        var type = typeof(T);
        
        return AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t))
            .Where(t => t is { IsInterface: false, IsAbstract: false });
    }
}