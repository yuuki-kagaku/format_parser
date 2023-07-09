namespace FormatParser.PE;

public record PeFileFormatInfo(Bitness Bitness, Architecture Architecture, bool IsManaged) : IFileFormatInfo
{
    public bool Equals(IFileFormatInfo? other) => other is PeFileFormatInfo peData && this.Equals(peData);
    
    public string ToPrettyString()
    {
        return $"Mach-O: {Architecture} {Bitness} managed: {IsManaged}";
    }
}