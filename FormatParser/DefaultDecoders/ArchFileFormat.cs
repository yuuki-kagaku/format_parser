namespace FormatParser.DefaultDecoders;

public record ArchFileFormat : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other)
    {
        return other is ArchFileFormat;
    }

    public string ToPrettyString()
    {
        return "application/x-archive";
    }
}