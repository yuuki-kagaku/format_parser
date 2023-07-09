using FormatParser.Text;

namespace FormatParser;

public class FormatDecoder
{
    private readonly IBinaryFormatDecoder[] binaryDecoders;
    private readonly CompositeTextFormatDecoder compositeTextFormatDecoder;
    private readonly FormatDecoderSettings settings;

    public FormatDecoder(IBinaryFormatDecoder[] binaryDecoders, CompositeTextFormatDecoder compositeTextFormatDecoder, FormatDecoderSettings settings)
    {
        this.binaryDecoders = binaryDecoders;
        this.compositeTextFormatDecoder = compositeTextFormatDecoder;
        this.settings = settings;
    }

    public async Task<IFileFormatInfo> DecodeFile(string file)
    {
        await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, settings.BufferSize);
        var deserializer = new Deserializer(fileStream);

        var result = await TryDecodeAsBinaryAsync(deserializer);
        if (result != null)
            return result;

        var buffer = await deserializer.ReadBytesAsync(settings.TextParserSettings.SampleSize);
        var inMemoryDeserializer = new InMemoryDeserializer(buffer);
        result = compositeTextFormatDecoder.TryDecode(inMemoryDeserializer);

        if (result != null)
            return result;

        return new UnknownFileFormatInfo();
    }

    private async Task<IFileFormatInfo?> TryDecodeAsBinaryAsync(Deserializer deserializer)
    {
        foreach (var binaryFormatDecoder in binaryDecoders)
        {
            try
            {
                var result = await binaryFormatDecoder.TryDecodeAsync(deserializer);
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