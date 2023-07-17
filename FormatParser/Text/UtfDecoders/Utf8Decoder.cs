using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf8Decoder : IUtfDecoder
{
    private readonly CodepointChecker codepointChecker;
    private readonly CodepointConverter codepointConverter;
    private readonly TextFileParsingSettings settings;

    private static readonly byte[] bom = { 0xEF, 0xBB, 0xBF };

    public Utf8Decoder(CodepointConverter codepointConverter, TextFileParsingSettings settings)
    {
        this.codepointChecker = CodepointChecker.IllegalC0AndC1Controls(CodepointCheckerSettings.Default);
        this.codepointConverter = codepointConverter;
        this.settings = settings;
    }
    
    public string[] CanReadEncodings { get; } = { WellKnownEncodings.Utf8BOM, WellKnownEncodings.Utf8NoBOM };

    public string? RequiredEncodingAnalyzer { get; } = null;
    
    public DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;

    public bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)]  out string? encoding)
    {
        var originalOffset = binaryReader.Offset;
        var haveBom = TryFindBom(binaryReader);

        if (!haveBom)
            binaryReader.Offset = originalOffset;

        var processedChars = 0;
        encoding = null;

        while (TryGetNextCodepoint(binaryReader, out var codepoint))
        {
            if (!codepointChecker.IsValidCodepoint(codepoint))
                return false;

            if (processedChars < settings.SampleSize)
            {
                codepointConverter.Convert(codepoint, stringBuilder);
                processedChars++;
            }
        }

        encoding = haveBom ? WellKnownEncodings.Utf8BOM : WellKnownEncodings.Utf8NoBOM;
        return true;
    }

    private static bool TryFindBom(InMemoryBinaryReader binaryReader)
    {
        if (!binaryReader.CanRead(3 * sizeof(byte)))
            return false;
        
        var b = binaryReader.ReadByte();
        if (b != bom[0])
            return false;

        b = binaryReader.ReadByte();
        if (b != bom[1])
            return false;
        
        b = binaryReader.ReadByte();
        if (b != bom[2])
            return false;

        return true;
    }

    private bool TryGetNextCodepoint(InMemoryBinaryReader binaryReader, out uint result)
    {
        result = 0;
        if (!binaryReader.TryReadByte(out var b))
            return false;
        
        if (b < 0x80)
        {
            result = b;
            return true;
        }

        if (b < 0xC0)
        {
            throw new Exception();
        }

        int size;

        if (b < 0xE0)
        {
            result = (uint)b & 0x1F;
            size = 2;
        }
        else if (b < 0xF0)
        {
            result = (uint)b & 0x0F; 
            size = 3;
        }
        else if (b < 0xF8)
        {
            result = (uint)b & 0x07; 
            size = 4;
        }
        else
            throw new Exception();

        for (var i = 1; i < size; i++)
        {
            if (!binaryReader.TryReadByte(out b))
                if (settings.CrashAtSplitCharAtEnd)
                    throw new Exception();
                else
                    return false;

            result <<= 6;

            result |= (uint)b & 0b111111;
        }

        return true;
    }
}