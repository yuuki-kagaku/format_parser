using ConsoleApp1;
using FormatParser;
using FormatParser.Helpers;
using FormatParser.Test.Helpers;
using FormatParser.Text;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = new QualityCheckerSettings();
        
        var directory = args[0];

        var codepointConverter = new CodepointConverter();
        var textParserSettings = TextParserSettings.Default;
        
        var nonUnicodeDecoders = new ITextDecoder[] {new Windows1251Decoder(textParserSettings)};
        var languageAnalyzers = new ITextAnalyzer[] {new RuDictionaryTextAnalyzer()};
        
        var utfDecoders = new IUtfDecoder[]
        {
            new Utf8Decoder(codepointConverter, textParserSettings),
            new Utf16LeDecoder(codepointConverter, textParserSettings),
            new Utf16BeDecoder(codepointConverter, textParserSettings),
        };
        
        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            utfDecoders, 
            nonUnicodeDecoders,
            languageAnalyzers,
            textParserSettings);
        
        var textDetectionComparer = new TextDetectionComparer(compositeTextFormatDecoder, settings);
        
        var state = new QualityCheckerState();
        DiscoverFiles(directory, state, textDetectionComparer);

        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine($"Total amount of mismatch (excluding false detections of '{settings.Utility}') : {state.MatchMismatches}");
        Console.WriteLine($"Text files according '{settings.Utility}' : {state.TextFilesAccordingToFileCommand}");
        Console.WriteLine($"Text files according to FormatParser : {state.TextFilesAccordingToFormatParser}");
        Console.WriteLine();
        Console.WriteLine($"False negative text file detections of '{settings.Utility}' (this files excluded from every statistics above) : {state.FalseNegativesOfFileCommand}");
    }

    private static void DiscoverFiles(string directory, QualityCheckerState state, TextDetectionComparer textDetectionComparer)
    {
        try
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.IsSymlink())
                    continue;
                
                if (fileInfo.IsEmpty())
                    continue;
                
                textDetectionComparer.ProcessFile(file, state);
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
        
        try
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                if (new FileInfo(subDirectory).IsSymlink())
                    continue;
                
                DiscoverFiles(subDirectory, state, textDetectionComparer);
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}