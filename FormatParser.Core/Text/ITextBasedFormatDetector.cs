using FormatParser.Domain;

namespace FormatParser.Text;

public interface ITextBasedFormatDetector
{
    IFileFormatInfo? TryMatchFormat(string text, EncodingInfo encodingInfo);
}