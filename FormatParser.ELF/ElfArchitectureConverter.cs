namespace FormatParser.ELF;

public static class ElfArchitectureConverter
{
    public static Architecture Convert(ushort architecture)
    {
        return architecture switch
        {
            ELFConstants.EM_X86_64 => Architecture.Amd64,
            ELFConstants.EM_386 => Architecture.i386,
            ELFConstants.EM_PPC => Architecture.PowerPC,
            ELFConstants.EM_PPC64 => Architecture.ppc64,
            ELFConstants.EM_68K => Architecture.m68k,
            ELFConstants.EM_SPARC => Architecture.Sparc,
            ELFConstants.EM_RISC => Architecture.RiscV,
            ELFConstants.EM_MIPS => Architecture.mips,
            ELFConstants.EM_ARM => Architecture.Arm,
            ELFConstants.EM_AARCH64 => Architecture.Arm64,
            ELFConstants.EM_S390 => Architecture.s390,
            _ => Architecture.Unknown
        };
    }
}