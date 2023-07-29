using FormatParser.Domain;
using FormatParser.Helpers.BinaryReader;
using FormatParser.Text;

namespace FormatParser;

public class FormatDetector
{
    private readonly IBinaryFormatDetector[] binaryDetectors;
    private readonly ITextBasedFormatDetector[] textBasedFormatDetectors;
    private readonly CompositeTextFormatDecoder compositeTextFormatDecoder;
    private readonly TextFileParsingSettings settings;

    public FormatDetector(IBinaryFormatDetector[] binaryDetectors, ITextBasedFormatDetector[] textBasedFormatDetectors, CompositeTextFormatDecoder compositeTextFormatDecoder, TextFileParsingSettings settings)
    {
        this.binaryDetectors = binaryDetectors;
        this.textBasedFormatDetectors = textBasedFormatDetectors;
        this.compositeTextFormatDecoder = compositeTextFormatDecoder;
        this.settings = settings;
    }

    public async Task<IFileFormatInfo> DetectAsync(Stream stream)
    {
        var streamingBinaryReader = new StreamingBinaryReader(stream, Endianness.BigEndian);

        var result = await TryDetectBinaryFormatAsync(streamingBinaryReader);

        if (result != null)
            return result;

        streamingBinaryReader.Offset = 0;
        var buffer = await streamingBinaryReader.TryReadArraySegmentAsync(settings.SampleSize);

        result = TryDetectTextBasedFormat(buffer);
  
        return result ?? new UnknownFileFormatInfo();
    }
    
    private async Task<IFileFormatInfo?> TryDetectBinaryFormatAsync(StreamingBinaryReader streamingBinaryReader)
    {
        foreach (var binaryFormatDetector in binaryDetectors)
        {
            try
            {
                streamingBinaryReader.Offset = 0;
                var result = await binaryFormatDetector.TryDetectAsync(streamingBinaryReader);
                
                if (result != null)
                    return result;
            }
            catch
            {
                // ignored
            }
        }

        return null;
    }
    
    private IFileFormatInfo? TryDetectTextBasedFormat(ArraySegment<byte> buffer)
    {
        if (!compositeTextFormatDecoder.TryDecode(buffer, out var encoding, out var textSample))
            return null;

        var detectionResult = TryMatchTextBasedFormat(textSample, encoding);
        
        return detectionResult ?? new TextFileFormatInfo(TextFileFormatInfo.DefaultTextType, encoding);
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
                // ignored
            }
        }

        return null;
    }
}