using FormatParser.Domain;
using FormatParser.Helpers.BinaryReader;
using FormatParser.Text;

namespace FormatParser;

public class FormatDetector
{
    private readonly IBinaryFormatDetector[] binaryDetectors;
    private readonly CompositeTextFormatDecoder compositeTextFormatDecoder;
    private readonly TextFileParsingSettings settings;

    public FormatDetector(IBinaryFormatDetector[] binaryDetectors, CompositeTextFormatDecoder compositeTextFormatDecoder, TextFileParsingSettings settings)
    {
        this.binaryDetectors = binaryDetectors;
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
        if (!compositeTextFormatDecoder.TryDecode(buffer, out var textFileFormatInfo))
            return null;

        return textFileFormatInfo;
    }
}