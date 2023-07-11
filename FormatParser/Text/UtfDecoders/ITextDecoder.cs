using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FormatParser.Text;

public interface ITextDecoder
{
    bool TryDecode(InMemoryBinaryReader binaryReader, StringBuilder stringBuilder, [NotNullWhen(true)] out string? encoding, out DetectionProbability detectionProbability);
    
    string? RequiredLanguageAnalyzer { get; }
}