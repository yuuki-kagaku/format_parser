using FormatParser.Domain;

namespace FormatParser.ELF;

public static class ElfArchitectureConverter
{
    public static Architecture Convert(ushort architecture, Bitness bitness, Endianness endianness) => architecture switch
    {
        ELFConstants.EM_X86_64 => Architecture.Amd64,
        ELFConstants.EM_386 => Architecture.I386,
        ELFConstants.EM_PPC => endianness switch
        {
            Endianness.BigEndian => Architecture.PowerPcBigEndian,
            Endianness.LittleEndian => Architecture.PowerPcLittleEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(endianness))
        },
        ELFConstants.EM_PPC64 => Architecture.Ppc64,
        ELFConstants.EM_68K => Architecture.M68K,
        ELFConstants.EM_SPARC => Architecture.Sparc,
        ELFConstants.EM_RISC => Architecture.RiscV,
        ELFConstants.EM_MIPS => endianness switch
        {
            Endianness.BigEndian => Architecture.MipsBigEndian,
            Endianness.LittleEndian => Architecture.MipsLittleEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(endianness))
        },
        ELFConstants.EM_ARM => Architecture.Arm,
        ELFConstants.EM_AARCH64 => Architecture.Arm64,
        ELFConstants.EM_S390 => bitness switch
        {
            Bitness.Bitness64 => Architecture.S390x,
            Bitness.Bitness32 => Architecture.S390,
            _ => throw new ArgumentOutOfRangeException(nameof(bitness), "Unsupported bitness for S390 architecture.")
        },
        _ => Architecture.Unknown
    };
}