using FormatParser.ArgumentParser;
using FormatParser.Helpers;
using FormatParser.Text;
using FormatParser.Text.Decoders;
using FormatParser.Text.TextAnalyzers;
using FormatParser.Windows1251;
using FormatParser.Xml;

namespace FormatParser.QualityChecker;

public static class EntryPoint
{
    public static void Main(string[] args)
    {
        var settings = ParseSettings(args);
        
        var textAnalyzers = new ITextAnalyzer[]
        {
            new AsciiCharactersTextAnalyzer(),
            new UTF16Heuristics(),
            new RuDictionaryTextAnalyzer(),
            new HeaderTextAnalyzer()
        };

        var textFileParsingSettings = new TextFileParsingSettings() {AllowC1ControlsForUtf = false};
        
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
            textAnalyzers,
            new ITextBasedFormatDetector[] { new XmlDecoder() });
        
        var textDetectionComparer = new TextDetectionComparer(compositeTextFormatDecoder, settings);
        
        var state = new QualityCheckerState();
        DiscoverFiles(settings.Directory ?? throw new ArgumentNullException(nameof(settings.Directory)), state, textDetectionComparer);

        PrintState(state, settings);
    }
    
    private static QualityCheckerSettings ParseSettings(string[] args)
    {
        var argsParser = new ArgumentParser<QualityCheckerSettings>()
            .WithPositionalArgument((settings, arg) => settings.Directory = arg)
            .OnNamedParameter("command", (settings, arg) => settings.Utility = arg, false)
            .OnNamedParameter("command-arg", (settings, arg) => settings.UtilityArgs = arg, false)
            .OnNamedParameter("buffer-size", (settings, arg) => settings.BufferSize = arg, false)
            .OnNamedParameter("not-print-mismatches", (settings) => settings.PrintMismatchedFilesInfo = false, false);

        return argsParser.Parse(args);
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