namespace FormatParser.Text;

public class Windows1251Decoder : OneByteEncodingTextDecoder
{
    public Windows1251Decoder( TextParserSettings settings) 
        : base(CodepointChecker.IllegalC0Controls(CodepointCheckerSettings.Default with {AdditionalInvalidCodepoints = new uint []{152}}), settings)
    {

    }

    public override char ConvertCodepoint(byte codepoint) => Windows1251Codepage.ConversionMap[codepoint];

    public override string Encoding { get; } = "Windows-1251";

    public override string? RequiredEncodingAnalyzer { get; } = "ru";
}