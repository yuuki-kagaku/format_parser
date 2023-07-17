namespace FormatParser.Text.Encoding;

public record TextDecodingResult(ArraySegment<char> Chars, EncodingData Encoding);