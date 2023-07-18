using FormatParser.Domain;

namespace FormatParser.ELF;

public static class ElfArchitectureConverter
{
    public static Architecture Convert(ushort architecture)
    {
        return architecture switch
        {
            ELFConstants.EM_X86_64 => Architecture.Amd64,
            ELFConstants.EM_386 => Architecture.I386,
            ELFConstants.EM_PPC => Architecture.PowerPC,
            ELFConstants.EM_PPC64 => Architecture.Ppc64,
            ELFConstants.EM_68K => Architecture.M68K,
            ELFConstants.EM_SPARC => Architecture.Sparc,
            ELFConstants.EM_RISC => Architecture.RiscV,
            ELFConstants.EM_MIPS => Architecture.Mips,
            ELFConstants.EM_ARM => Architecture.Arm,
            ELFConstants.EM_AARCH64 => Architecture.Arm64,
            ELFConstants.EM_S390 => Architecture.S390,
            _ => Architecture.Unknown
        };
    }
}