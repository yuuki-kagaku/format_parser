using System.Text;

namespace FormatParser.Text.Encoding;

public class AlwaysFallDecoderFallbackBuffer : DecoderFallbackBuffer
{
    public override bool Fallback(byte[] bytesUnknown, int index) => throw new DecoderFallbackException();

    public override char GetNextChar() => char.MinValue;

    public override bool MovePrevious() => false;

    public override int Remaining => 0;
}