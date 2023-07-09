namespace FormatParser.PE;

public record PeFileFormatInfo(Bitness Bitness, Architecture Architecture, bool IsManaged) : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is PeFileFormatInfo peFileFormatInfo && this.Equals(peFileFormatInfo);
    
    public string ToPrettyString()
    {
        return $"PE: {Architecture} {Bitness} managed: {IsManaged}";
    }
}