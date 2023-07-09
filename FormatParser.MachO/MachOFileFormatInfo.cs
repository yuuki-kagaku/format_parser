namespace FormatParser.MachO;

public record MachOFileFormatInfo(Endianess Endianess, Bitness Bitness, Architecture Architecture, bool Signed) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other)
    {
        if (other is not MachOFileFormatInfo textFileFormatInfo)
            return false;

        return this.Equals(textFileFormatInfo);
    }

    public string ToPrettyString()
    {
        return $"Mach-O: {Architecture} {Bitness} {Endianess} signed:{Signed}";
    }
}