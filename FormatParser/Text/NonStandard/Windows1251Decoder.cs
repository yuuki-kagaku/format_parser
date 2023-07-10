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

    public bool TryDecode(InMemoryDeserializer deserializer, StringBuilder stringBuilder, out string? encoding, out DetectionProbability detectionProbability)
    {
        var processedChars = 0;
        
        while (deserializer.CanRead(sizeof(byte)))
        {
            var codepoint = deserializer.ReadByte();
                
            if (!IsValidCodepoint(codepoint))
            {
                encoding = null;
                detectionProbability = DetectionProbability.No;
                return false;
            }

            if (processedChars < settings.SampleSize)
            {
                stringBuilder.Append(Windows1251Codepage.ConversionDictionary[codepoint]);
                processedChars++;
            }
        }

        encoding = Encoding;
        detectionProbability = DetectionProbability.Low;
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