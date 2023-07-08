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

    public async Task<IData> DecodeFile(string file)
    {
        await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, settings.BufferSize);
        var deserializer = new Deserializer(fileStream);

        var result = await TryDecodeAsBinary(deserializer);
        if (result != null)
            return result;

        var buffer = await deserializer.ReadBytes(settings.TextParserSettings.SampleSize);
        var inMemoryDeserializer = new InMemoryDeserializer(buffer);
        result = compositeTextFormatDecoder.TryDecodeAsync(inMemoryDeserializer);

        if (result != null)
            return result;

        return new UnknownData();
    }

    private async Task<IData?> TryDecodeAsBinary(Deserializer deserializer)
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