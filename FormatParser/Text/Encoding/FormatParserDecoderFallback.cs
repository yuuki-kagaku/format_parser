using System.Text;

namespace FormatParser.Text.Encoding;

public class FormatParserDecoderFallback : DecoderFallback
{
    private readonly int inputSize;
    private readonly int maxSizeOfCharacter;

    private FormatParserDecoderFallback(int inputSize, int maxSizeOfCharacter)
    {
        this.inputSize = inputSize;
        this.maxSizeOfCharacter = maxSizeOfCharacter;
    }

    public override DecoderFallbackBuffer CreateFallbackBuffer() => new FormatParserDecoderFallbackBuffer(inputSize, maxSizeOfCharacter);

    public override int MaxCharCount => 0;

    public static FormatParserDecoderFallback DoNotFailAtEndOfInput(int inputSize, int maxSizeOfCharacter) => new (inputSize, maxSizeOfCharacter);
}