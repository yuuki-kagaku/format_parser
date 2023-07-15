namespace FormatParser;

public record UnknownFileFormatInfo : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is UnknownFileFormatInfo;

    public string ToPrettyString() => "Unknown";
}