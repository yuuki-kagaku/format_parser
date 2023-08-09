using FormatParser.Domain;
using FormatParser.Text.Helpers;

namespace FormatParser.Text;

public class HeaderTextAnalyzer : ITextAnalyzer
{
    public DetectionProbability AnalyzeProbability(string text, EncodingInfo encoding, out EncodingInfo? clarifiedEncoding)
    {
        clarifiedEncoding = null;
        
        var firstNewLine = text.IndexOfAny(new []{'\r', '\n', ControlCharacters.NEL});

        if (firstNewLine == -1 || firstNewLine < 2)
            return DetectionProbability.No;

        if (!text.Take(firstNewLine - 1).All(CharacterHelper.IsAscii))
            return DetectionProbability.No;

        foreach (var (first, last) in Patterns)
        {
            if (text[0] == first && text[firstNewLine - 1] == last)
                return DetectionProbability.Lowest;
        }

        return DetectionProbability.No;
    }
    
    private static (char Firts, char Last)[] Patterns = { ('<', '>'), ('[', ']') };
    public string[] AnalyzerIds { get; } = { "header_analyzer" };
}