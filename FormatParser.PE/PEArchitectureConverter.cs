using FormatParser.Domain;

namespace FormatParser.PE;

public static class PEArchitectureConverter
{
    public static (Architecture, Bitness) Convert(uint i) =>
        i switch
        {
            PEConstants.IMAGE_FILE_MACHINE_I386 => (Architecture.I386, Bitness.Bitness32),
            PEConstants.IMAGE_FILE_MACHINE_AMD64 => (Architecture.Amd64, Bitness.Bitness64),
            PEConstants.IMAGE_FILE_MACHINE_IA64 => (Architecture.Ia64, Bitness.Bitness64),
            PEConstants.IMAGE_FILE_MACHINE_ARM64 => (Architecture.Arm64, Bitness.Bitness64),
            PEConstants.IMAGE_FILE_MACHINE_ARM => (Architecture.Arm, Bitness.Bitness32),
            PEConstants.IMAGE_FILE_MACHINE_THUMB => (Architecture.Arm, Bitness.Bitness32),
            PEConstants.IMAGE_FILE_MACHINE_ARMNT => (Architecture.Arm, Bitness.Bitness32),
            _ => throw new FormatParserException("Unknown architecture")
        };
}