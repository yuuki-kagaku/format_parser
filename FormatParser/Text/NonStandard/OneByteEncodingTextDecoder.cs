using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public abstract class OneByteEncodingTextDecoder : ITextDecoder
{
    private readonly CodepointChecker codepointChecker;
    private readonly TextFileParsingSettings settings;

    protected OneByteEncodingTextDecoder(CodepointChecker codepointChecker, TextFileParsingSettings settings)
    {
        this.codepointChecker = codepointChecker;
        this.settings = settings;
    }

    public bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding)
    {
        var processedChars = 0;
        
        while (binaryReader.CanRead(sizeof(byte)))
        {
            var codepoint = binaryReader.ReadByte();
                
            if (!IsValidCodepoint(codepoint))
            {
                encoding = null;
                return false;
            }

            if (processedChars < settings.SampleSize)
            {
                stringBuilder.Append(ConvertCodepoint(codepoint));
                processedChars++;
            }
        }

        encoding = Encoding;
        return true;
    }
    
    public abstract string Encoding { get; }
    
    public DetectionProbability DefaultDetectionProbability { get; } = DetectionProbability.Lowest;

    public abstract string? RequiredEncodingAnalyzer { get; }

    public abstract char ConvertCodepoint(byte codepoint);
    
    private bool IsValidCodepoint(uint codepoint)
    {
        if (!codepointChecker.IsValidCodepoint(codepoint))
            return false;

        return true;
    }

}