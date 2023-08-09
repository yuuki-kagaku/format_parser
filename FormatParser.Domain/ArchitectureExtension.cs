namespace FormatParser.Domain;

public static class ArchitectureExtension
{
    public static string ToStringWithoutBitness(this Architecture architecture) => architecture switch
    {
        Architecture.Unknown => "Unknown",
        Architecture.I386 => "i386",
        Architecture.Amd64 => "Amd64",
        Architecture.Arm => "Arm",
        Architecture.Arm64 => "Arm",
        Architecture.Ia64 => "IA-64",
        Architecture.M68K => "M68K",
        Architecture.PowerPcBigEndian => "PowerPC",
        Architecture.PowerPcLittleEndian => "PowerPC",
        Architecture.Ppc64 => "PowerPC",
        Architecture.Sparc => "Sparc",
        Architecture.MipsLittleEndian => "Mips",
        Architecture.MipsBigEndian => "Mips",
        Architecture.S390 => "IBM System/390",
        Architecture.S390x => "z/Architecture",
        Architecture.RiscV => "Risc-V",
        _ => throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null)
    };
}