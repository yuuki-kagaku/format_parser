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
        try
        {
            if (!compositeTextFormatDecoder.TryDecode(buffer, out var encoding, out var textSample))
                return null;
            
            if (TryMatchTextBasedFormat(textSample, out var type, out var formatEncoding))
                return new TextFileFormatInfo(type, encoding with {Name = formatEncoding ?? encoding.Name});

            return new TextFileFormatInfo(DefaultTextType, encoding);
        }
        catch
        {
            return null; 
        }
    }
    
    private bool TryMatchTextBasedFormat(string header, [NotNullWhen(true)] out string? type, out string? encoding)
    {
        foreach (var detector in textBasedFormatDetectors)
        {
            try
            {
                if (detector.TryMatchFormat(header, out encoding))
                {
                    type = detector.MimeType;
                    return true;
                }
            }
            catch
            {
            }
        }

        (type, encoding) = (null, null);
        return false;
    }
    
    public static string DefaultTextType => "text/plain";
}