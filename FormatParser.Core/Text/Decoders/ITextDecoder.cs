namespace FormatParser.Text.Decoders;

public interface ITextDecoder
{ 
    TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes);
    
    string[]? RequiredEncodingAnalyzers { get; }
    DetectionProbability DefaultDetectionProbability { get; }

    IEnumerable<char> GetInvalidCharacters { get; }
}