using FormatParser.Domain;

namespace FormatParser.Text.Decoders;

public interface ITextDecoder
{ 
    TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes);
    
    string[]? RequiredEncodingAnalyzers { get; }
    DetectionProbability DefaultDetectionProbability { get; }

    bool IsCharacterValid(char c);
    
    bool SupportBom { get; }
    
    string EncodingName { get; }
    
    bool RequireTextBasedFormatMatch { get; }
}