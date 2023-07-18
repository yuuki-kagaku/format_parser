namespace FormatParser.Domain;

public sealed record UnknownFileFormatInfo : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is UnknownFileFormatInfo;

    public string ToPrettyString() => "Unknown";
}