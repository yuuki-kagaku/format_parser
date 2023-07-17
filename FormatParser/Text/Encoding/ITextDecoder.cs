namespace FormatParser.Text.Encoding;

public interface ITextDecoder
{ 
    TextDecodingResult? TryDecodeText(ArraySegment<byte> bytes, char[] chars);
    
    string? RequiredEncodingAnalyzer { get; }
    DetectionProbability DefaultDetectionProbability { get; }

    HashSet<uint> GetInvalidCharacters { get; }
}