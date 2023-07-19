using FormatParser.Domain;

namespace FormatParser.MachO;

public record MachOFileFormatInfo(Endianness Endianness, Bitness Bitness, Architecture Architecture, bool Signed) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is MachOFileFormatInfo machOFileFormatInfo && Equals(machOFileFormatInfo);

    public string ToPrettyString() => 
        $"Mach-O: {ToPrettyStringWithoutPrefix()}";
    
    public string ToPrettyStringWithoutPrefix() => 
        $"{Architecture.ToStringWithoutBitness()}/{Bitness.ToPrettyString()} {Endianness.ToPrettyString()} signed: {FormatSigned()}";

    private string FormatSigned() => Signed ? "yes" : "no"; 
}