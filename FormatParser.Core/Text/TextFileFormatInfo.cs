using FormatParser.Domain;

namespace FormatParser.Text;

public record TextFileFormatInfo(string MimeType, EncodingInfo Encoding) : IFileFormatInfo
{
    private static readonly StringComparer StringComparer = StringComparer.InvariantCultureIgnoreCase;
    public virtual bool Equals(TextFileFormatInfo? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return StringComparer.Equals(MimeType,other.MimeType) && StringComparer.Equals(Encoding , other.Encoding);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StringComparer.GetHashCode(MimeType), StringComparer.GetHashCode(Encoding));
    }

    public virtual bool Equals(IFileFormatInfo? other) => other is TextFileFormatInfo textFileFormatInfo && Equals(textFileFormatInfo);

    public string ToPrettyString() => $"{MimeType} ; {Encoding.ToPrettyString()}";
    
    public static string DefaultTextType => "text/plain";
}