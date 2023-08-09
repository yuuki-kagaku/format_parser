using FormatParser.Domain;
using FormatParser.Text.Decoders;

namespace FormatParser.Text;

public abstract class NonUtfDecoder : DecoderBase
{
    public override bool SupportBom => false;

    protected override EncodingInfo EncodingWithoutBom => EncodingInfo;
    
    protected override EncodingInfo EncodingWithBom => throw new NotSupportedException();
    
    private EncodingInfo EncodingInfo  => new(EncodingName, Endianness.NotAllowed, false);
}