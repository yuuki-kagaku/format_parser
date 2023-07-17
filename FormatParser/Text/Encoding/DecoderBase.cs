using System.Text;

namespace FormatParser.Text.Encoding;

public abstract class DecoderBase : ITextDecoder
{
    public TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes, char[] chars)
    {
        if (bytes.Count < MinimalSizeOfInput)
            return null;
        
        var decoder = GetDecoder(bytes.Count);
        try
        {
            var numberOfChars = decoder.GetChars(bytes, chars, true);

            if (SupportBom && chars[0] == UnicodeHelper.Bom)
                return new TextDecodingResult(new ArraySegment<char>(chars, 1, numberOfChars - 1), EncodingWithBom);
           
            return new TextDecodingResult(new ArraySegment<char>(chars, 0, numberOfChars), EncodingWithoutBom);
        }
        catch (DecoderFallbackException)
        {
            return null;
        }
    }

    protected abstract Decoder GetDecoder(int inputSize);
    
    public abstract int MinimalSizeOfInput { get; }

    public abstract bool SupportBom { get; }
    
    public abstract EncodingData EncodingWithBom { get; }
    
    public abstract EncodingData EncodingWithoutBom { get; }

    public abstract HashSet<char> GetInvalidCharacters { get; }

    public abstract string? RequiredEncodingAnalyzer { get; }
    
    public abstract DetectionProbability DefaultDetectionProbability { get; }
}