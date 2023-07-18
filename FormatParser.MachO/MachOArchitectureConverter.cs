using FormatParser.Domain;

namespace FormatParser.MachO;

public static class MachOArchitectureConverter
{
    public static Architecture Convert(int type) => type switch
    {
        MachOConstants.CPU_TYPE_I386 => Architecture.I386,
        MachOConstants.CPU_TYPE_X86_64 => Architecture.Amd64,
        MachOConstants.CPU_TYPE_MC680x0 => Architecture.M68K,
        MachOConstants.CPU_TYPE_MIPS => Architecture.Mips,
        MachOConstants.CPU_TYPE_ARM => Architecture.Arm,
        MachOConstants.CPU_TYPE_SPARC => Architecture.Sparc,
        MachOConstants.CPU_TYPE_POWERPC => Architecture.PowerPC,
        MachOConstants.CPU_TYPE_POWERPC64 => Architecture.Ppc64,
        _ => Architecture.Unknown
    };
}