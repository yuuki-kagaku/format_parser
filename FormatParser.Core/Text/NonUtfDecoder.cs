using FormatParser.Domain;
using FormatParser.Text.Decoders;

namespace FormatParser.Text;

public abstract class NonUtfDecoder : DecoderBase
{
    protected abstract EncodingInfo EncodingInfo { get; }
    
    protected override bool SupportBom => false;

    public override EncodingInfo EncodingWithoutBom => EncodingInfo;
    
    protected override EncodingInfo EncodingWithBom => throw new NotSupportedException();
}