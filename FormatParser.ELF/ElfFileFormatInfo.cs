namespace FormatParser.ELF;

public record ElfFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, string? Interpreter) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is ElfFileFormatInfo textFileFormatInfo && this.Equals(textFileFormatInfo);

    public string ToPrettyString()
    {
        return $"ELF: {Architecture} {Bitness} {Endianness} {Interpreter}";
    }
}