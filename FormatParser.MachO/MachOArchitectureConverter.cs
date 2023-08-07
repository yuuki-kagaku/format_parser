using FormatParser.Domain;

namespace FormatParser.MachO;

public static class MachOArchitectureConverter
{
    public static Architecture Convert(int type, Endianness endianness) => type switch
    {
        MachOConstants.CPU_TYPE_I386 => Architecture.I386,
        MachOConstants.CPU_TYPE_X86_64 => Architecture.Amd64,
        MachOConstants.CPU_TYPE_MC680x0 => Architecture.M68K,
        MachOConstants.CPU_TYPE_MIPS => endianness switch
        {
            Endianness.BigEndian => Architecture.MipsBigEndian,
            Endianness.LittleEndian => Architecture.MipsLittleEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(endianness))
        },
        MachOConstants.CPU_TYPE_ARM => Architecture.Arm,
        MachOConstants.CPU_TYPE_SPARC => Architecture.Sparc,
        MachOConstants.CPU_TYPE_POWERPC => endianness switch
    {
        Endianness.BigEndian => Architecture.PowerPcBigEndian,
        Endianness.LittleEndian => Architecture.PowerPcLittleEndian,
        _ => throw new ArgumentOutOfRangeException(nameof(endianness))
    },
        MachOConstants.CPU_TYPE_POWERPC64 => Architecture.Ppc64,
        _ => Architecture.Unknown
    };
}