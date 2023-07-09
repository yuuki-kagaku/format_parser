namespace FormatParser.MachO;

public record MachOFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, bool Signed) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is MachOFileFormatInfo textFileFormatInfo && this.Equals(textFileFormatInfo);

    public string ToPrettyString()
    {
        return $"Mach-O: {Architecture} {Bitness} {Endianness} signed:{Signed}";
    }
}