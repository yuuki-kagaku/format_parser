using FormatParser.Domain;
using FormatParser.Helpers.BinaryReader;
using FormatParser.Text;

namespace FormatParser;

public class FormatDecoder
{
    private readonly IBinaryFormatDetector[] binaryDecoders;
    private readonly TextFileProcessor textFileProcessor;
    private readonly TextFileParsingSettings settings;

    public FormatDecoder(IBinaryFormatDetector[] binaryDecoders, TextFileProcessor textFileProcessor, TextFileParsingSettings settings)
    {
        this.binaryDecoders = binaryDecoders;
        this.textFileProcessor = textFileProcessor;
        this.settings = settings;
    }

    public async Task<IFileFormatInfo> Decode(Stream stream)
    {
        var binaryReader = new StreamingBinaryReader(stream, Endianness.BigEndian);

        var result = await TryDecodeAsBinaryAsync(binaryReader);

        if (result != null)
            return result;

        binaryReader.Offset = 0;
        
        var buffer = await binaryReader.TryReadArraySegment(settings.SampleSize);

        result = textFileProcessor.TryProcess(buffer);
  
        if (result != null)
            return result;

        return new UnknownFileFormatInfo();
    }

    private async Task<IFileFormatInfo?> TryDecodeAsBinaryAsync(StreamingBinaryReader binaryReader)
    {
        foreach (var binaryFormatDecoder in binaryDecoders)
        {
            try
            {
                binaryReader.Offset = 0;
                var result = await binaryFormatDecoder.TryDetectAsync(binaryReader);
                
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
}