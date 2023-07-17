using System.Text;

namespace FormatParser.Text.Encoding;

public class FormatParserDecoderFallback : DecoderFallback
{
    private readonly int inputSize;
    private readonly int maxSizeOfCharacter;
    private readonly bool alwaysThrow;

    private FormatParserDecoderFallback(int inputSize, int maxSizeOfCharacter, bool alwaysThrow)
    {
        this.inputSize = inputSize;
        this.maxSizeOfCharacter = maxSizeOfCharacter;
        this.alwaysThrow = alwaysThrow;
    }

    public override DecoderFallbackBuffer CreateFallbackBuffer() => new FormatParserDecoderFallbackBuffer(inputSize, maxSizeOfCharacter);

    public override int MaxCharCount => 0;

    public static FormatParserDecoderFallback DoNotFailAtEndOfInput(int inputSize, int maxSizeOfCharacter) => new (inputSize, maxSizeOfCharacter, false);
    public static FormatParserDecoderFallback AlwaysThrow() => new (0, 0, true);
}