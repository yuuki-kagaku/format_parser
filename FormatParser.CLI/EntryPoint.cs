using System.Reflection;
using FormatParser.CLI.ArgumentParser;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.EncodingAnalyzers;
using FormatParser.TextBasedFormats;

namespace FormatParser.CLI;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = ParseSettings(args);

        LoadPlugins();
        
        var binaryDecoders = GetAllInstancesOf<IBinaryFormatDetector>().ToArray();

        var textAnalyzers = GetAllInstancesOf<ITextAnalyzer>().ToArray();
        
        var textFileParsingSettings = new TextFileParsingSettings();
        
        var textDecoders = GetAllInstancesOf<ITextDecoder, TextFileParsingSettings>(textFileParsingSettings).ToArray();

        var textBasedFormatDetectors = GetAllInstancesOf<ITextBasedFormatDetector>().ToArray();

        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            textDecoders, 
            textAnalyzers,
            settings.TextFileParsingSettings);

        var runner = new CLIRunner(
            new FileDiscoverer(new FileDiscovererSettings()), 
            new FormatDecoder(binaryDecoders, 
                new TextFileProcessor(textBasedFormatDetectors, compositeTextFormatDecoder), 
                settings.TextFileParsingSettings
            )
        );

        var cts = new CancellationTokenSource();
        var state = new FormatParserCliState();
        
        runner.Run(settings, state, cts.Token).GetAwaiter().GetResult();

        Console.WriteLine($"Run complete. Elapsed: {state.Stopwatch!.Elapsed.TotalMilliseconds}");
        Console.WriteLine();
        
        foreach (var (formatInfo, occurence) in state.Occurence.OrderByDescending(x => x.Value.Read()))
        {
            Console.WriteLine($"[{occurence.Read().ToString(),6}] : {formatInfo.ToPrettyString()}");
        }
    }

    private static FormatParserCliSettings ParseSettings(string[] args)
    {
        var argsParser = new ArgumentParser<FormatParserCliSettings>()
            .WithPositionalArgument((settings, arg) => settings.Directory = arg)
            .OnNamedParameter("parallel", ((settings, arg) => settings.DegreeOfParallelism = arg), false);

        return argsParser.Parse(args);
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