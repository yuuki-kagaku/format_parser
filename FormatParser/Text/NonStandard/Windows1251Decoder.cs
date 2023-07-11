using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public class Windows1251Decoder : ITextDecoder
{
    private readonly CodepointChecker codepointChecker;
    private readonly TextParserSettings settings;

    public Windows1251Decoder(CodepointChecker codepointChecker, TextParserSettings settings)
    {
        this.codepointChecker = codepointChecker;
        this.settings = settings;
    }

    public bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding, out DetectionProbability detectionProbability)
    {
        var processedChars = 0;
        
        while (binaryReader.CanRead(sizeof(byte)))
        {
            var codepoint = binaryReader.ReadByte();
                
            if (!IsValidCodepoint(codepoint))
            {
                (encoding, detectionProbability) = (null, DetectionProbability.No);
                return false;
            }

            if (processedChars < settings.SampleSize)
            {
                stringBuilder.Append(Windows1251Codepage.ConversionMap[codepoint]);
                processedChars++;
            }
        }

        (encoding, detectionProbability) = (Encoding, DetectionProbability.Low);
        return true;
    }

    private bool IsValidCodepoint(uint codepoint)
    {
        if (!codepointChecker.IsValidCodepoint(codepoint))
            return false;

        return true;
    }

    public static readonly string Encoding = "Windows-1251";

    public string? RequiredLanguageAnalyzer { get; } = "ru";
}