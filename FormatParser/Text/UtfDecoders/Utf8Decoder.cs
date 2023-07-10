using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Utf8Decoder : IUtfDecoder
{
    private readonly CodepointChecker codepointChecker;
    private readonly CodepointConverter codepointConverter;
    private readonly TextParserSettings settings;

    public Utf8Decoder(CodepointChecker codepointChecker, CodepointConverter codepointConverter, TextParserSettings settings)
    {
        this.codepointChecker = codepointChecker;
        this.codepointConverter = codepointConverter;
        this.settings = settings;
    }
    
    public string[] CanReadEncodings { get; } = { WellKnownEncodings.Utf8BOM, WellKnownEncodings.Utf8NoBOM };

    public string? RequiredLanguageAnalyzer { get; } = null;

    public bool TryDecode(InMemoryDeserializer deserializer, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding, out DetectionProbability detectionProbability)
    {
        var processedChars = 0;
        var onlyAsciiSymbols = true;
      
        while (TryGetNextCodepoint(deserializer, out var codepoint))
        {
            if (codepoint > 128)
                onlyAsciiSymbols = false;
                
            if (!codepointChecker.IsValidCodepoint(codepoint))
            {
                (encoding, detectionProbability) = (null, DetectionProbability.No);
                return false;
            }

            if (processedChars < settings.SampleSize)
            {
                codepointConverter.Convert(codepoint, stringBuilder);
                processedChars++;
            }
        }

        encoding = onlyAsciiSymbols ? WellKnownEncodings.ASCII : WellKnownEncodings.Utf8NoBOM;
        detectionProbability = onlyAsciiSymbols ? DetectionProbability.Medium : DetectionProbability.Low;
        return true;
    }

    private bool TryGetNextCodepoint(InMemoryDeserializer deserializer, out uint result)
    {
        result = 0;
        if (!deserializer.TryReadByte(out var b))
            return false;
        
        if (b < 0x80)
        {
            result = b;
            return true;
        }

        if (b < 0xC0)
            throw new Exception();
                    
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
            if  (!deserializer.TryReadByte(out b))
                if (settings.CrashAtSplitCharAtEnd)
                    throw new Exception();
                else
                    return false;
                        
            result <<= 6;
            result |= (uint)b & 0x6F;
        }

        return true;
    }
}