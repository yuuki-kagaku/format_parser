using FormatParser.Domain;

namespace FormatParser.ELF;

public record ElfFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, string? Interpreter) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is ElfFileFormatInfo elfFileFormatInfo && this.Equals(elfFileFormatInfo);

    public string ToPrettyString()
    {
        return $"ELF: {Architecture} {Bitness} {Endianness} {Interpreter}";
    }
}