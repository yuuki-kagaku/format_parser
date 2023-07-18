namespace FormatParser.Text.Decoders;

public record TextDecodingResult(ArraySegment<char> Chars, EncodingInfo Encoding);