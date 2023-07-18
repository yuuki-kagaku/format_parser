namespace FormatParser.Text.Decoders;

public interface ITextDecoder
{ 
    TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes);
    
    string? RequiredEncodingAnalyzer { get; }
    DetectionProbability DefaultDetectionProbability { get; }

    IEnumerable<char> GetInvalidCharacters { get; }
}