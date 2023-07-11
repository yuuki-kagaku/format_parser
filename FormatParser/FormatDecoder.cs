using FormatParser.Text;

namespace FormatParser;

public class FormatDecoder
{
    private readonly IBinaryFormatDecoder[] binaryDecoders;
    private readonly TextFileProcessor textFileProcessor;
    private readonly FormatDecoderSettings settings;

    public FormatDecoder(IBinaryFormatDecoder[] binaryDecoders, TextFileProcessor textFileProcessor, FormatDecoderSettings settings)
    {
        this.binaryDecoders = binaryDecoders;
        this.textFileProcessor = textFileProcessor;
        this.settings = settings;
    }

    public async Task<IFileFormatInfo> DecodeFile(string file)
    {
        await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, settings.BufferSize);
        var binaryReader = new StreamingBinaryReader(fileStream);

        var result = await TryDecodeAsBinaryAsync(binaryReader);
        
        if (result != null)
            return result;

        var buffer = await binaryReader.TryReadArraySegment(settings.TextParserSettings.SampleSize);
        var inMemoryBinaryReader = new InMemoryBinaryReader(buffer);
        result = textFileProcessor.TryProcess(inMemoryBinaryReader);

        if (result != null)
            return result;

        return new UnknownFileFormatInfo();
    }

    private async Task<IFileFormatInfo?> TryDecodeAsBinaryAsync(StreamingBinaryReader streamingBinaryReader)
    {
        foreach (var binaryFormatDecoder in binaryDecoders)
        {
            try
            {
                streamingBinaryReader.Offset = 0;
                var result = await binaryFormatDecoder.TryDecodeAsync(streamingBinaryReader);
                if (result != null)
                    return result;
            }
            catch (Exception e)
            {
            }
        }

        return null;
    }
}