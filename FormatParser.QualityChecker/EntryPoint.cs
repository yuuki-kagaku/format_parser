using ConsoleApp1;
using FormatParser;
using FormatParser.Helpers;
using FormatParser.Test.Helpers;
using FormatParser.Text;
using FormatParser.Text.Encoding;
using ITextDecoder = FormatParser.Text.Encoding.ITextDecoder;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = new QualityCheckerSettings();
        
        var directory = args[0];

        var codepointConverter = new CodepointConverter();
        
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
        
        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            utfDecoders, 
            nonUnicodeDecoders,
            languageAnalyzers,
            settings.TextFileParsingSettings);
        
        var textDetectionComparer = new TextDetectionComparer(compositeTextFormatDecoder, settings);
        
        var state = new QualityCheckerState();
        DiscoverFiles(directory, state, textDetectionComparer);

        PrintState(state, settings);
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

    private static void PrintState(QualityCheckerState state, QualityCheckerSettings settings)
    {
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine($"Total amount of mismatches (excluding false detections of '{settings.Utility}') : {state.MatchMismatches}");
        Console.WriteLine($"Text files according '{settings.Utility}' : {state.TextFilesAccordingToFileCommand}");
        Console.WriteLine($"Text files according to FormatParser : {state.TextFilesAccordingToFormatParser}");
        Console.WriteLine();
        Console.WriteLine($"False negative text file detections of '{settings.Utility}' (this files excluded from every statistics above) : {state.FalseNegativesOfFileCommand}");

    }
}