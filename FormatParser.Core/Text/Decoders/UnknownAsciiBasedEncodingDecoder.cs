using System.Text;
using FormatParser.Domain;
using EncodingInfo = FormatParser.Domain.EncodingInfo;

namespace FormatParser.Text.Decoders;

public class UnknownAsciiBasedEncodingDecoder : DecoderBase, ITextDecoder
{
    public UnknownAsciiBasedEncodingDecoder(TextFileParsingSettings settings)
    {
    }

    protected override Decoder GetDecoder(int inputSize) => new AsciiBasedEncodingDecoder();

    protected override IReadOnlySet<char> InvalidCharacters => new HashSet<char> { '\0' };

    protected override int MinimalSizeOfInput => 0;

    public override string EncodingName => "unknown-8bit";

    public override bool RequireTextBasedFormatMatch { get; } = true;

    public override bool SupportBom => true;
    protected override EncodingInfo EncodingWithBom => new (EncodingName, Endianness.NotAllowed, true);
    protected override EncodingInfo EncodingWithoutBom => new (EncodingName, Endianness.NotAllowed, false);

    public override string[]? RequiredEncodingAnalyzers { get; } = { "header_analyzer" };
    
    public override DetectionProbability DefaultDetectionProbability => DetectionProbability.No;
}