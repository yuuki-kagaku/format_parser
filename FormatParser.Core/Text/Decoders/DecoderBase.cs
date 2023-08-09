using System.Text;
using FormatParser.Domain;
using FormatParser.Text.Helpers;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public abstract class DecoderBase : ITextDecoder
{
    public TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes)
    {
        var decoder = GetDecoder(bytes.Count);
        try
        {
            var chars = new char[bytes.Count];
            var numberOfChars = decoder.GetChars(bytes, chars, true);
            
            if (SupportBom && chars[0] == CharacterHelper.Bom)
                return new TextDecodingResult(new ArraySegment<char>(chars, 1, numberOfChars - 1), EncodingWithBom);
           
            if (bytes.Count < MinimalSizeOfInput)
                return null;
            
            return new TextDecodingResult(new ArraySegment<char>(chars, 0, numberOfChars), EncodingWithoutBom);
        }
        catch (DecoderFallbackException)
        {
            return null;
        }
    }
    
    public abstract bool SupportBom { get; }

    public abstract string EncodingName { get; }
    
    public abstract bool RequireTextBasedFormatMatch { get; }
    
    public bool IsCharacterValid(char c) => !InvalidCharacters.Contains(c);

    protected abstract IReadOnlySet<char> InvalidCharacters { get; }
    
    public abstract string[]? RequiredEncodingAnalyzers { get; }
    
    public abstract DetectionProbability DefaultDetectionProbability { get; }

    protected abstract Decoder GetDecoder(int inputSize);

    protected abstract int MinimalSizeOfInput { get; }

    protected abstract EncodingInfo EncodingWithBom { get; }
    
    protected abstract EncodingInfo EncodingWithoutBom { get; }
}