namespace FormatParser;

public record UnknownFileFormatInfo : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other)
    {
        return other is UnknownFileFormatInfo;
    }

    public string ToPrettyString()
    {
        return "unknown";
    }
}