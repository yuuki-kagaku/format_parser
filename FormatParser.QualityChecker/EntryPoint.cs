using FormatParser.Helpers;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.EncodingAnalyzers;
using FormatParser.Text.UtfDecoders;
using FormatParser.Windows1251;

namespace FormatParser.QualityChecker;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = new QualityCheckerSettings();
        
        var directory = args[0];

        var languageAnalyzers = new ITextAnalyzer[]
        {
            new AsciiCharactersTextAnalyzer(),
            new UTF16Heuristics(),
            new RuDictionaryTextAnalyzer()
        };

        var textFileParsingSettings = new TextFileParsingSettings();
        
        var decoders = new ITextDecoder[]
        {
            new Utf8Decoder(textFileParsingSettings),
            new Utf16LeDecoder(textFileParsingSettings),
            new Utf16BeDecoder(textFileParsingSettings),
            new Utf32LeDecoder(textFileParsingSettings),
            new Utf32BeDecoder(textFileParsingSettings),
            new Windows1251Decoder(settings.TextFileParsingSettings)
        };
        
        var compositeTextFormatDecoder = new CompositeTextFormatDecoder(
            decoders, 
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
        Console.WriteLine($"Text files according to FormatParser.Core : {state.TextFilesAccordingToFormatParser}");
        Console.WriteLine();
        Console.WriteLine($"False negative text file detections of '{settings.Utility}' (this files excluded from every statistics above) : {state.FalseNegativesOfFileCommand}");

    }
}