namespace FormatParser.Text.Decoders;

public interface ITextDecoder
{ 
    TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes, char[] chars);
    
    string? RequiredEncodingAnalyzer { get; }
    DetectionProbability DefaultDetectionProbability { get; }

    HashSet<char> GetInvalidCharacters { get; }
}