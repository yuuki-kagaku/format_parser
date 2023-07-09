using FormatParser;
using FormatParser.CLI;
using FormatParser.ELF;
using FormatParser.MachO;
using FormatParser.PE;
using FormatParser.Text;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var directory = args[0];

        var textChecker = new TextChecker();
        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        var formatParserSetting = new FormatParserCliSettings
        {
            Directory = directory
        };
        
        var runner = new CLIRunner(
            new FileDiscoverer(), 
            new FormatDecoder(new IBinaryFormatDecoder[]{new ElfDecoder(), new MachODecoder(), new PEDecoder()}, 
                new CompositeTextFormatDecoder(new IUtfDecoder[]
                {
                    new Utf8Decoder(textChecker, codepointConverter, textParserSettings),
                    new Utf16Decoder(textChecker, codepointConverter, textParserSettings),
                }, new ITextBasedFormatDetector[]
                {
                    new XmlDecoder()
                }, textParserSettings),new FormatDecoderSettings(textParserSettings, 8192)
                )
            );

        var cts = new CancellationTokenSource();
        var state = new ForamatParserCliState();
        runner.Run(formatParserSetting, state, cts.Token).GetAwaiter().GetResult();
        
        foreach (var (formatInfo, occurence) in state.Occurence)
        {
            Console.WriteLine($"[{occurence.Read().ToString(),6}] : {formatInfo}");
        }
    }
}