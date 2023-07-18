using System.Diagnostics.CodeAnalysis;

namespace FormatParser.MachO;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class MachOConstants
{
    #region Architecture
    
    public const int CPU_TYPE_MC680x0 = 6;
    public const int CPU_TYPE_I386 = 7;
    public const int CPU_TYPE_MIPS = 8;
    public const int CPU_TYPE_ARM = 12;
    public const int CPU_TYPE_SPARC = 14;
    public const int CPU_TYPE_POWERPC = 18;
    public const int CPU_ARCH_ABI64 = 0x1000000;
    public const int CPU_TYPE_X86_64 = CPU_TYPE_I386 | CPU_ARCH_ABI64;
    public const int CPU_TYPE_POWERPC64 = CPU_TYPE_POWERPC | CPU_ARCH_ABI64;

    #endregion

    #region Load Commands
    
    public const uint LC_CODE_SIGNATURE = 0x1d;
    
    #endregion
}