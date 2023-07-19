using FormatParser.Domain;

namespace FormatParser.ELF;

public record ElfFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, string? Interpreter) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is ElfFileFormatInfo elfFileFormatInfo && Equals(elfFileFormatInfo);

    public string ToPrettyString()
    {
        return $"ELF: {Architecture.ToStringWithoutBitness()}/{Bitness.ToPrettyString()} {Endianness.ToPrettyString()}{PrintInterpreter()}";
    }

    private string PrintInterpreter() => Interpreter == null ? String.Empty : $" interpreter: {Interpreter}";
}