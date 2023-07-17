using System.Reflection;
using System.Text.Json;
using FormatParser.Text;
using FormatParser.Text.Encoding;
using FormatParser.Windows1251;

namespace FormatParser.CLI;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = ParseSettings(args);

        LoadPlugins();
        
        var binaryDecoders = GetAllInstancesOf<IBinaryFormatDecoder>().ToArray();
        
        var nonUnicodeDecoders = new ITextDecoder[] {new Windows1251Decoder(settings.TextFileParsingSettings)};
        var languageAnalyzers = new ITextAnalyzer[] {new RuDictionaryTextAnalyzer()};
        
        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(),
            new Utf16LeDecoder(),
            new Utf16BeDecoder(),
            new Utf32LeDecoder(),
            new Utf32BeDecoder(),
        };
        
        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
            new XmlDecoder()
        };

        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            utfDecoders, 
            nonUnicodeDecoders,
            languageAnalyzers,
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
        const string pluginDirectory = "./plugins/";

        if (!Directory.Exists(pluginDirectory))
            return;

        foreach (var directory in Directory.GetDirectories(pluginDirectory))
        {
            var plugins = Directory
                .EnumerateFiles(directory)
                .Where(x => x.EndsWith(".dll"));

            foreach (var dll in plugins)
            {
                var a = Assembly.LoadFrom(dll);
                AppDomain.CurrentDomain.Load(a.GetName());
            }
        }
    }

    private static IEnumerable<T> GetAllInstancesOf<T>()
    {
        var type = typeof(T);
        var types = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t))
            .Where(t => !t.IsInterface || !t.IsAbstract);

        foreach (var t in types)
            yield return (T)Activator.CreateInstance(t)!;
    }
}