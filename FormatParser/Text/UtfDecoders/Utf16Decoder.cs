using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public abstract class Utf16Decoder : IUtfDecoder
{
    private readonly CodepointChecker codepointChecker;
    private readonly CodepointConverter codepointConverter;
    private readonly TextParserSettings settings;

    protected Utf16Decoder(CodepointChecker codepointChecker, CodepointConverter codepointConverter, TextParserSettings settings)
    {
        this.codepointChecker = codepointChecker;
        this.codepointConverter = codepointConverter;
        this.settings = settings;
    }

    public abstract bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding,out DetectionProbability detectionProbability);

    public abstract string[] CanReadEncodings { get; }

    public string? RequiredLanguageAnalyzer { get; } = null;

    protected ushort BOMInNativeEndianess = 0xFFFE;
    
    protected bool TryParseInternal(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, Endianness endianness, out bool foundBom, out DetectionProbability probability)
    {
        binaryReader.SetEndianness(endianness);
        
        stringBuilder.Clear();
        var processedChars = 0;
        var onlyAsciiChars = true;
        foundBom = false;
        
        while (binaryReader.CanRead(sizeof(ushort)))
        {
            var codepoint = GetNextCodepoint(binaryReader);
            if (codepoint > 127)
                onlyAsciiChars = false;

            if (!codepointChecker.IsValidCodepoint(codepoint))
            {
                probability = DetectionProbability.No;
                return false;
            }

            if (processedChars == 0 && codepoint == BOMInNativeEndianess)
            {
                foundBom = true;
                continue;
            }

            if (processedChars < settings.SampleSize)
            {
                codepointConverter.Convert(codepoint, stringBuilder);
                processedChars++;
            }
        }

        probability = onlyAsciiChars ? DetectionProbability.Medium : DetectionProbability.Low;
        return true;
    }

    private static uint GetNextCodepoint(InMemoryBinaryReader binaryReader)
    {
        var current = binaryReader.ReadUShort();
        
        if (current >= 0xd800 && current < 0xDC00)
        {
            if (!binaryReader.TryReadUShort(out var next))
                throw new BinaryReaderException("Unexpected end of utf16 string.");
            return ((current & (uint)0x3FF) << 10) + (next & (uint)0x3FF) + (uint)0x10000;
        }

        return current;
    }
}