using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.Xml;

public record XmlFileFormatInfo : TextFileFormatInfo
{
    public XmlFileFormatInfo(string MimeType, EncodingInfo Encoding) : base(MimeType, Encoding)
    {
    }

    protected override bool AlwaysPrintBom { get; } = true;
}