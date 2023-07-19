using System.Diagnostics.CodeAnalysis;
using FormatParser.Domain;

namespace FormatParser.Text;

public class TextFileProcessor
{
    private readonly ITextBasedFormatDetector[] textBasedFormatDetectors;
    private readonly CompositeTextFormatDecoder compositeTextFormatDecoder;

    public TextFileProcessor(ITextBasedFormatDetector[] textBasedFormatDetectors, CompositeTextFormatDecoder compositeTextFormatDecoder)
    {
        this.textBasedFormatDetectors = textBasedFormatDetectors;
        this.compositeTextFormatDecoder = compositeTextFormatDecoder;
    }

    public IFileFormatInfo? TryProcess(ArraySegment<byte> buffer)
    {
     
        if (!compositeTextFormatDecoder.TryDecode(buffer, out var encoding, out var textSample))
            return null;

        var detectionResult = TryMatchTextBasedFormat(textSample, encoding);
        if (detectionResult != null)
            return detectionResult;
            
        return new TextFileFormatInfo(DefaultTextType, encoding);
    }
    
    private IFileFormatInfo? TryMatchTextBasedFormat(string header, EncodingInfo encoding)
    {
        foreach (var detector in textBasedFormatDetectors)
        {
            try
            {
                var detectionResult = detector.TryMatchFormat(header, encoding);
                if (detectionResult != null)
                    return detectionResult;
            }
            catch
            {
            }
        }

        return null;
    }
    
    public static string DefaultTextType => "text/plain";
}