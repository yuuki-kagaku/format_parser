namespace FormatParser.ELF;

internal static class ELFConstants
{
    public static byte[] ElfMagicBytes = {0x7F, (byte)'E', (byte)'L', (byte)'F'};
    
    #region Endianness
    
    public const byte ELFDATA2LSB = 1;
    public const byte ELFDATA2MSB = 2;

    #endregion

    #region Bitness

    public const byte ELFCLASS32 = 1;
    public const byte ELFCLASS64 = 2;

    #endregion
    
    # region Segment types
    
    public const int PT_INTERP = 3;
    
    #endregion

    #region Architecture

    public const ushort EM_X86_64 = 62;

    #endregion
    
}