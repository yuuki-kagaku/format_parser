namespace FormatParser.ELF;

public record ElfFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, string? Interpreter) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other)
    {
        if (other is not ElfFileFormatInfo textFileFormatInfo)
            return false;

        return this.Equals(textFileFormatInfo);
    }

    public string ToPrettyString()
    {
        return $"ELF: {Architecture} {Bitness} {Endianness} {Interpreter}";
    }
}