namespace FormatParser.Text;

public record TextFileFormatInfo(string MimeType, string Encoding) : IFileFormatInfo
{
 
    public virtual bool Equals(IFileFormatInfo? other)
    {
        if (other is not TextFileFormatInfo textFileFormatInfo)
            return false;

        return this.Equals(textFileFormatInfo);
    }

    public string ToPrettyString()
    {
        return $"{MimeType} / {Encoding}";
    }
}