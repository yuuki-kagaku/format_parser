namespace FormatParser.Text;

public record TextFileFormatInfo(string MimeType, string Encoding) : IFileFormatInfo
{
 
    public virtual bool Equals(IFileFormatInfo? other) => other is TextFileFormatInfo textFileFormatInfo && this.Equals(textFileFormatInfo);

    public string ToPrettyString()
    {
        return $"{MimeType} / {Encoding}";
    }
}