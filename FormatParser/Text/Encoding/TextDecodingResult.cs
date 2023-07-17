namespace FormatParser.Text.Encoding;

public record TextDecodingResult(ArraySegment<char> Chars, string Encoding);