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

    public const ushort EM_SPARC = 2;//	SPARC
    public const ushort EM_386 = 3; 	//Intel 80386
    public const ushort EM_68K = 4; //Motorola 68000
    public const ushort EM_PPC = 20; //	PowerPC
    public const ushort EM_PPC64 = 21;//	64-bit PowerPC
    public const ushort EM_ARM = 40;
    public const ushort EM_S390 = 22;

    public const ushort EM_X86_64 = 62;
    public const ushort EM_AARCH64 = 183;
    public const ushort EM_RISC = 243;
    
    public const ushort EM_MIPS = 8;
    
    #endregion

}