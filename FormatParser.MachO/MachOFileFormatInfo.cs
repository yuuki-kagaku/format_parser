using FormatParser.Domain;

namespace FormatParser.MachO;

public record MachOFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, bool Signed) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is MachOFileFormatInfo machOFileFormatInfo && Equals(machOFileFormatInfo);

    public string ToPrettyString() => 
        $"Mach-O: {Architecture.ToStringWithoutBitness()}/{Bitness.ToPrettyString()} {Endianness.ToPrettyString()} signed:{Signed}";
}