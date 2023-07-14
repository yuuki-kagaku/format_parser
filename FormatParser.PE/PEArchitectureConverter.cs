namespace FormatParser.PE;

public static class PEArchitectureConverter
{
    public static (Architecture, Bitness) Convert(uint i)
    {
        var bitness = (i & 0x00FF) == 0x64 ? Bitness.Bitness64: Bitness.Bitness32;

        return (GetArchitecture(), bitness);
        
        Architecture GetArchitecture() => i switch
        {
            PEConstants.IMAGE_FILE_MACHINE_I386 => Architecture.i386,
            PEConstants.IMAGE_FILE_MACHINE_AMD64 => Architecture.Amd64,
            PEConstants.IMAGE_FILE_MACHINE_IA64 => Architecture.ia64,
            PEConstants.IMAGE_FILE_MACHINE_ARM64 => Architecture.Arm64,
            _ => Architecture.Unknown
        };
    }
}