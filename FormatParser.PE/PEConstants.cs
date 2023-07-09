namespace FormatParser.PE;

internal static class PEConstants
{
    #region Directory Entries

    public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
    public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;
    public const int SizeOfDataDirectory = sizeof(uint) + sizeof(uint);

    #endregion

    #region OptionalHeaderMagicNumbers

    public const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
    public const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;
    public const ushort IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x107;

    #endregion

    #region Image Header
    
    public const uint ImageHeaderMagicNumber = 0x00004550; // PE00

    #endregion

    #region Architectures

    public const ushort IMAGE_FILE_MACHINE_I386 = 0x014c;
    public const ushort IMAGE_FILE_MACHINE_IA64 = 0x0200;
    public const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;
    public const ushort IMAGE_FILE_MACHINE_ARM64 = 0xAA64;

    #endregion
    
}