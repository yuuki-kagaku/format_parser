using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Xml;

public record XmlFileFormatInfo(string MimeType, EncodingInfo Encoding) : TextFileFormatInfo(MimeType, Encoding)
{
    protected override bool AlwaysPrintBom { get; } = true;
}