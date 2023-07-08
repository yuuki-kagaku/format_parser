using FormatParser.Text;

namespace FormatParser;

public record FormatDecoderSettings(TextParserSettings TextParserSettings, int BufferSize);