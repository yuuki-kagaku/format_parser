namespace FormatParser.Text;

public record TextFileFormatInfo(string MimeType, string Encoding) : IFileFormatInfo
{
    private static readonly StringComparer stringComparer = StringComparer.InvariantCultureIgnoreCase;
    public virtual bool Equals(TextFileFormatInfo? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return stringComparer.Equals(MimeType,other.MimeType) && stringComparer.Equals(Encoding , other.Encoding);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(stringComparer.GetHashCode(MimeType), stringComparer.GetHashCode(Encoding));
    }

    public virtual bool Equals(IFileFormatInfo? other) => other is TextFileFormatInfo textFileFormatInfo && this.Equals(textFileFormatInfo);

    public string ToPrettyString() => $"{MimeType} ; {Encoding}";
}