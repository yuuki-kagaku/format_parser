using System.Reflection;
using FormatParser.Text;

namespace FormatParser.CLI;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var directory = args[0];

        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        var formatParserSetting = new FormatParserCliSettings
        {
            Directory = directory
        };

        LoadPlugins();
        
        var binaryDecoders = GetAllInstancesOf<IBinaryFormatDecoder>().ToArray();
        
        var nonUnicodeDecoders = new ITextDecoder[] {new Windows1251Decoder(textParserSettings)};
        var languageAnalyzers = new ILanguageAnalyzer[] {new RussianLanguageAnalyzer()};
        
        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(codepointConverter, textParserSettings),
            new Utf16LeDecoder(codepointConverter, textParserSettings),
            new Utf16BeDecoder(codepointConverter, textParserSettings),
        };
        
        var textBasedFormatDetectors = new ITextBasedFormatDetector[]
        {
            new XmlDecoder()
        };

        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            utfDecoders, 
            nonUnicodeDecoders,
            languageAnalyzers,
            textParserSettings);

        var runner = new CLIRunner(
            new FileDiscoverer(), 
            new FormatDecoder(binaryDecoders, 
                new TextFileProcessor(textBasedFormatDetectors, compositeTextFormatDecoder), 
                new FormatDecoderSettings(textParserSettings, 8192)
            )
        );

        var cts = new CancellationTokenSource();
        var state = new ForamatParserCliState();
        runner.Run(formatParserSetting, state, cts.Token).GetAwaiter().GetResult();
        
        foreach (var (formatInfo, occurence) in state.Occurence.OrderByDescending(x => x.Value.Read()))
        {
            Console.WriteLine($"[{occurence.Read().ToString(),6}] : {formatInfo.ToPrettyString()}");
        }
    }

    private static void LoadPlugins()
    {
        const string pluginDirectory = "./plugins/";
        
        if (Directory.Exists(pluginDirectory))
            return;

        var plugins = Directory
            .EnumerateFiles(pluginDirectory)
            .Where(x => x.EndsWith(".dll"));
        
        foreach (var dll in plugins)
        {
            var a = Assembly.LoadFrom(dll);
            AppDomain.CurrentDomain.Load(a.GetName());
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
        {
            yield return (T)Activator.CreateInstance(t)!;
        }
    }
}