using FormatParser.Test.Helpers;
using FormatParser.Text;

namespace FormatParser.QualityChecker;

public class TextDetectionComparer
{
    private readonly CompositeTextFormatDecoder compositeTextFormatDecoder;
    private readonly QualityCheckerSettings settings;
    private readonly byte[] buffer;
    private static readonly string[] TextFormats =
    {
        "text/",
        "application/json",
        "application/postscript", 
        "image/svg+xml",
        "application/x-wine-extension-ini",
        "application/x-setupscript",
        "image/x-portable-bitmap", 
        "image/x-portable-graymap",
        "image/x-portable-pixmap",
        "application/pgp-keys", 
        "image/x-xpmi",
    };

    public TextDetectionComparer(CompositeTextFormatDecoder compositeTextFormatDecoder, QualityCheckerSettings settings)
    {
        this.compositeTextFormatDecoder = compositeTextFormatDecoder;
        this.settings = settings;
        buffer = new byte[settings.BufferSize];

    }

    public void ProcessFile(string file, QualityCheckerState state)
    {
        try
        {
            if (!File.Exists(file))
                return;

            var commandOutput = ShellRunner.RunCommand(settings.Utility, $@"{settings.UtilityArgs} ""{file}""");

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            
            var size = fileStream.Read(buffer, 0, buffer.Length);
            
            if (ShouldSkip(commandOutput, size))
                return;
            
            var isTextAccordingToFormatParser = IsTextAccordingToFormatParser(new ArraySegment<byte>(buffer, 0, size));

            var isTextFileAccordingToFileCommand = IsTextFile(commandOutput, file);

            if (isTextFileAccordingToFileCommand)
                state.TextFilesAccordingToFileCommand++;

            if (isTextAccordingToFormatParser)
                state.TextFilesAccordingToFormatParser++;

            var correctedIsTextFileAccordingToFileCommand = isTextFileAccordingToFileCommand;
            var isFalseNegative = IsFalseNegativeTextDetection(buffer, size);
            if (isFalseNegative)
            {
                correctedIsTextFileAccordingToFileCommand = false;
                state.FalseNegativesOfFileCommand++;
            }

            var isFalsePositive = IsFalsePositiveTextDetection(buffer, size, file);
            if (isFalsePositive)
            {
                correctedIsTextFileAccordingToFileCommand = true;
                state.FalsePositivesOfFileCommand++;
            }

            if (correctedIsTextFileAccordingToFileCommand != isTextAccordingToFormatParser)
            {
                state.MatchMismatches++;
                if (settings.PrintMismatchedFilesInfo)
                    Console.WriteLine(@$"Fount text detection missmatch: {file} | According to Format parser it is {IsText(isTextAccordingToFormatParser)}, but according to '{settings.Utility}' it is not, an it is: {commandOutput}");
            }
        }

        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
    }
    
    private static bool IsFalseNegativeTextDetection(byte[] buffer, int size)
    {
        if (size == 1 && buffer[0] == 0x0A)
            return true;
        
        return false;
    }
    
    private static bool IsFalsePositiveTextDetection(byte[] buffer, int size, string filename)
    {
        if (filename.StartsWith("/usr/lib/firmware/rtl_bt/") && filename.EndsWith(".bin"))
            return true;
            
        return false;
    }
    
    private bool IsTextAccordingToFormatParser(ArraySegment<byte> buffer)
    {
        return compositeTextFormatDecoder.TryDecode(buffer, out _);
    }

    private static bool IsTextFile(string commandFileOutput, string file)
    {
        return TextFormats.Any(x => commandFileOutput.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }
    
    private static string IsText(bool isText) => isText ? "text" : "binary";

    private static bool ShouldSkip(string commandOutput, int size) =>
        size == 0 ||
        commandOutput.Contains("application/x-archive", StringComparison.InvariantCultureIgnoreCase) && size == "!<arch>\n".Length;
}