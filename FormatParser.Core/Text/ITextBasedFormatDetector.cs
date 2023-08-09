using FormatParser.Domain;

namespace FormatParser.Text;

public interface ITextBasedFormatDetector
{
    TextFileFormatInfo? TryMatchFormat(string text, EncodingInfo encodingInfo, out bool encodingIsConclusive);
}