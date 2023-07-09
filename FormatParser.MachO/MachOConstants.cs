namespace FormatParser.MachO;

internal static class MachOConstants
{
    #region Architecture
    
    public const int CPU_TYPE_I386 = 7;
    public const int CPU_ARCH_ABI64 = 0x1000000;
    public const int CPU_TYPE_X86_64 = CPU_TYPE_I386 | CPU_ARCH_ABI64;

    #endregion

    #region Load Commands
    public const uint LC_CODE_SIGNATURE = 0x1d;
    #endregion
}