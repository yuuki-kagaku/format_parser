namespace FormatParser.Domain;

public static class ArchitectureExtension
{
    public static string ToStringWithoutBitness(this Architecture architecture)
    {
        return architecture switch
        {
            Architecture.Unknown => "Unknown",
            Architecture.I386 => "i386",
            Architecture.Amd64 => "Amd64",
            Architecture.Arm => "Arm",
            Architecture.Arm64 => "Arm",
            Architecture.Ia64 => "IA-64",
            Architecture.M68K => "M68K",
            Architecture.PowerPC => "PowerPC",
            Architecture.Ppc64 => "PowerPC",
            Architecture.Sparc => "Sparc",
            Architecture.Mips => "Mips",
            Architecture.S390 => "IBM System Z",
            Architecture.RiscV => "Risc-V",
            _ => throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null)
        };
    }
}