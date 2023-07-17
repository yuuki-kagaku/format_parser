using System.Text;

namespace FormatParser.Text.Encoding;

public sealed class FormatParserDecoderFallbackBuffer : DecoderFallbackBuffer
{
  private readonly int inputSize;
  private readonly int maxSizeOfCharacter;

  public FormatParserDecoderFallbackBuffer(int inputSize, int maxSizeOfCharacter)
  {
    this.inputSize = inputSize;
    this.maxSizeOfCharacter = maxSizeOfCharacter;
  }

  public override bool Fallback(byte[] bytesUnknown, int index)
  {
    if (index > inputSize - maxSizeOfCharacter)
      return false;

    throw new DecoderFallbackException();
  }

  public override char GetNextChar() => char.MinValue;

  public override bool MovePrevious() => false;

  public override int Remaining => 0;
}