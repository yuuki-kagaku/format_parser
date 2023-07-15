using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public abstract class Utf16Decoder : IUtfDecoder
{
    private readonly CodepointChecker codepointChecker;
    private readonly CodepointConverter codepointConverter;
    private readonly TextParserSettings settings;

    protected Utf16Decoder(CodepointConverter codepointConverter, TextParserSettings settings)
    {
        this.codepointChecker = CodepointChecker.IllegalC0AndC1Controls(CodepointCheckerSettings.Default);
        this.codepointConverter = codepointConverter;
        this.settings = settings;
    }

    public abstract bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding);

    public abstract string[] CanReadEncodings { get; }

    public string? RequiredEncodingAnalyzer { get; } = null;

    public DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.No;

    private ushort bomInNativeEndianess = 0xFFFE;
    
    protected bool TryParseInternal(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, Endianness endianness, out bool foundBom)
    {
        foundBom = false;
        if (settings.CrashIfUtf16InputIsNotEven && ((binaryReader.Length - binaryReader.Offset) / 2 == 1))
            return false;
        
        binaryReader.SetEndianness(endianness);
        var processedChars = 0;
        
        while (binaryReader.CanRead(sizeof(ushort)))
        {
            var codepoint = GetNextCodepoint(binaryReader);

            if (!codepointChecker.IsValidCodepoint(codepoint))
                return false;

            if (processedChars == 0 && codepoint == bomInNativeEndianess)
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