using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class CompositeTextFormatDecoder
{
    private readonly IUtfDecoder[] utfDecoders;
    private readonly ITextBasedFormatDetector[] textBasedFormatDetectors;
    private readonly TextParserSettings settings;
    private readonly List<char> buffer;
    private readonly StringBuilder stringBuilder;
    private readonly Dictionary<Encoding, IUtfDecoder> utfDecodersByEncoding;
    
    
    public CompositeTextFormatDecoder(IUtfDecoder[] utfDecoders, ITextBasedFormatDetector[] textBasedFormatDetectors, TextParserSettings settings)
    {
        this.utfDecoders = utfDecoders;
        utfDecodersByEncoding = Enum
            .GetValues<Encoding>()
            .Select(e => (Encoding: e, Decoder: utfDecoders.FirstOrDefault(d => d.MatchEncoding(e))))
            .Where(x => x.Decoder != null)
            .ToDictionary(x => x.Encoding, x => x.Decoder!);
            
        this.textBasedFormatDetectors = textBasedFormatDetectors;
        this.settings = settings;
        buffer = new List<char>(settings.SampleSize);
        stringBuilder = new StringBuilder(settings.SampleSize);
    }
    
    public IFileFormatInfo? TryDecode(InMemoryDeserializer deserializer)
    {
        if (TryFindBom(deserializer, out var encoding, out var bom))
        {
            var decoder = utfDecodersByEncoding[encoding];
            deserializer.Offset = bom.Length;

            if (!decoder.TryDecode(deserializer, buffer, out _))
                throw new Exception("File is corrupted.");
        }
        else
        {
            if (!TryDecodeAsUtf(deserializer, out encoding))
                return null;
        }

        var header = GetString(buffer);
        if (TryMatchTextBasedFormat(header, out var type, out var formatEncoding))
            return new TextFileFormatInfo(type, formatEncoding ?? encoding.ToString());
            
        return new TextFileFormatInfo(DefaultTextType, encoding.ToString());
    }

    private bool TryDecodeAsUtf(InMemoryDeserializer deserializer, out Encoding encoding)
    {
        foreach (var utfDecoder in utfDecoders)
        {
            try
            {
                deserializer.Offset = 0;
                buffer.Clear();

                if (utfDecoder.TryDecode(deserializer, buffer, out encoding))
                    return true;
            }
            catch (Exception e)
            {
            }
        }

        encoding = default;
        return false;
    }

    private string GetString(List<char> buffer)
    {
        stringBuilder.Clear();
        foreach (var c in buffer)
            stringBuilder.Append(c);

        return stringBuilder.ToString();
    }

    private bool TryMatchTextBasedFormat(string header, [NotNullWhen(true)] out string? type, [NotNullWhen(true)] out string? encoding)
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

    private static bool TryFindBom(InMemoryDeserializer deserializer, out Encoding result, [NotNullWhen(true)] out byte[]? bom)
    {
        deserializer.Offset = 0;
        var firstBytes = deserializer.ReadBytes(4);
        
        foreach (var (magicBytes, encoding) in boms)
        {
            if (magicBytes.SequenceEqual(firstBytes))
            {
                result = encoding;
                bom = magicBytes;
                return true;
            }
        }

        result = default;
        bom = null;
        return false;
    }

    private static string DefaultTextType => "text/plain";

    private static readonly (byte[], Encoding)[] boms =
    {
        (new byte[]{0xFF, 0xFE, 0x00, 0x00}, Encoding.UTF32LeBom),
        (new byte[]{0x00, 0x00, 0xFE, 0xFF}, Encoding.UTF32LeBom),
        (new byte[]{0xEF, 0xBB, 0xBF}, Encoding.Utf8BOM),
        (new byte[]{0xFE, 0xFF}, Encoding.UTF16BeBom),
        (new byte[]{0xFF, 0xFE}, Encoding.UTF16LeBom),
    };
    
}