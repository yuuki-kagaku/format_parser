using System.Reflection;
using FormatParser;
using FormatParser.CLI;
using FormatParser.Text;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var directory = args[0];

        var textChecker = new CodepointChecker();
        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        var formatParserSetting = new FormatParserCliSettings
        {
            Directory = directory
        };

        var wellKnownDlls = new string[]
        {
            "../../../../FormatParser.ELF/bin/Debug/net7.0/FormatParser.ELF.dll",
            "../../../../FormatParser.PE/bin/Debug/net7.0/FormatParser.PE.dll",
            "../../../../FormatParser.MachO/bin/Debug/net7.0/FormatParser.MachO.dll",
        };
        foreach (var dll in wellKnownDlls)
        {
            var a = Assembly.LoadFrom(dll);
            AppDomain.CurrentDomain.Load(a.GetName());
        }
        
        var binaryDecoders = GetAllClassesInstances<IBinaryFormatDecoder>().ToArray();
        
        var nonUnicodeDecoders = new ITextDecoder[] {new Windows1251Decoder(new CodepointChecker(), textParserSettings)};
        var languageAnalyzers = new ILanguageAnalyzer[] {new RussianLanguageAnalyzer()};
        
        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(textChecker, codepointConverter, textParserSettings),
            new Utf16LeDecoder(textChecker, codepointConverter, textParserSettings),
            new Utf16BeDecoder(textChecker, codepointConverter, textParserSettings),
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
            Console.WriteLine($"[{occurence.Read().ToString(),6}] : {formatInfo}");
        }
    }

    private static IEnumerable<T> GetAllClassesInstances<T>()
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
            yield return (T)Activator.CreateInstance(t);
        }
    }
}