using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly IUtfDecoder[] utfDecoders;
    private readonly ITextBasedFormatDetector[] textBasedFormatDetectors;
    private readonly TextParserSettings settings;
    private readonly Dictionary<string, IUtfDecoder> utfDecodersByEncoding;
    
    public CompositeTextFormatDecoder(IUtfDecoder[] utfDecoders, ITextBasedFormatDetector[] textBasedFormatDetectors, TextParserSettings settings)
    {
        this.utfDecoders = utfDecoders;
        utfDecodersByEncoding = utfDecoders
            .SelectMany(d => d.CanReadEncodings.Select(e => (Decoder: d, Encoding: e)))
            .ToDictionary(x => x.Encoding, x => x.Decoder);
            
        this.textBasedFormatDetectors = textBasedFormatDetectors;
        this.settings = settings;
    }
    
    public IFileFormatInfo? TryDecode(InMemoryDeserializer deserializer)
    {
        var stringBuilder = new StringBuilder(settings.SampleSize);
        if (TryFindUtfBom(deserializer, out var encoding, out var bom))
        {
            var decoder = utfDecodersByEncoding[encoding];
            deserializer.Offset = bom.Length;

            if (!decoder.TryDecode(deserializer, stringBuilder, out _))
                throw new Exception("File is corrupted.");
        }
        else
        {
            if (!TryDecodeAsUtf(deserializer, stringBuilder, out encoding))
                return null;
        }

        var header = stringBuilder.ToString();
        if (TryMatchTextBasedFormat(header, out var type, out var formatEncoding))
            return new TextFileFormatInfo(type, formatEncoding ?? encoding.ToString());
            
        return new TextFileFormatInfo(DefaultTextType, encoding.ToString());
    }

    private bool TryDecodeAsUtf(InMemoryDeserializer deserializer, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding)
    {
        foreach (var utfDecoder in utfDecoders)
        {
            try
            {
                deserializer.Offset = 0;
                stringBuilder.Clear();

                if (utfDecoder.TryDecode(deserializer, stringBuilder, out encoding))
                    return true;
            }
            catch (Exception e)
            {
            }
        }

        encoding = default;
        return false;
    }

    private bool TryMatchTextBasedFormat(string header, [NotNullWhen(true)] out string? type, out string? encoding)
    {
        foreach (var detector in textBasedFormatDetectors)
        {
            if (detector.TryMatchFormat(header, out encoding))
            {
                type = detector.MimeType;
                return true;
            }
        }

        type = null;
        encoding = null;
        return false;
    }

    private static bool TryFindUtfBom(InMemoryDeserializer deserializer, [NotNullWhen(true)] out string? encoding, [NotNullWhen(true)] out byte[]? bom)
    {
        deserializer.Offset = 0;
        var firstBytes = deserializer.ReadBytes(4);
        
        foreach (var (magicBytes, e) in boms)
        {
            if (magicBytes.SequenceEqual(firstBytes))
            {
                encoding = e;
                bom = magicBytes;
                return true;
            }
        }

        encoding = default;
        bom = null;
        return false;
    }

    private static string DefaultTextType => "text/plain";

    private static readonly (byte[], string)[] boms =
    {
        (new byte[]{0xFF, 0xFE, 0x00, 0x00}, WellKnownEncodings.UTF32LeBom),
        (new byte[]{0x00, 0x00, 0xFE, 0xFF}, WellKnownEncodings.UTF32LeBom),
        (new byte[]{0xEF, 0xBB, 0xBF}, WellKnownEncodings.Utf8BOM),
        (new byte[]{0xFE, 0xFF}, WellKnownEncodings.UTF16BeBom),
        (new byte[]{0xFF, 0xFE}, WellKnownEncodings.UTF16LeBom),
    };
    
}