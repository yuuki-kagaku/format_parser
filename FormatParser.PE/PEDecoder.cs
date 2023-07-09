namespace FormatParser.PE;

public class PEDecoder : IBinaryFormatDecoder
{
    public async Task<IFileFormatInfo?> TryDecodeAsync(Deserializer deserializer)
    {
        deserializer.Offset = 0;
        deserializer.SetEndianess(Endianess.LittleEndian);
        var dosHeader = await TryParseHeader(deserializer);

        if (dosHeader == null)
            return null;

        deserializer.Offset = dosHeader.Value.ExeOffset;

        var (architecture, bitness, sizeOfOptionalHeader) = await ReadImageFileHeaderAsync(deserializer);
        var isDotNet = await ReadOptionalHeader(deserializer, sizeOfOptionalHeader, bitness);

        return new PeFileFormatInfo(bitness, architecture, isDotNet);
    }

    private static async Task<(Architecture, Bitness, ushort SizeOfOptionalHeader)> ReadImageFileHeaderAsync(Deserializer deserializer)
    {
        EnsureCorrectImageHeaderMagicNumber(await deserializer.ReadUInt()); // Magic
        var (architecture, bitness) = ParseArchitecture(await deserializer.ReadUShort()); // Machine
        deserializer.SkipUShort(); // NumberOfSections
        deserializer.SkipUInt(); //  TimeDateStamp
        deserializer.SkipUInt(); //  PointerToSymbolTable
        deserializer.SkipUInt(); //  NumberOfSymbols
        var sizeOfOptionalHeader = await deserializer.ReadUShort(); // SizeOfOptionalHeader
        deserializer.SkipUShort(); // Characteristics

        return (architecture, bitness, sizeOfOptionalHeader);
    }

    private async Task<bool> ReadOptionalHeader(Deserializer deserializer, ushort size, Bitness bitness)
    {
        if (size < 224)
            return false;
        
        var magicNumber = await deserializer.ReadUShort(); // Magic
        EnsureCorrectOptionalHeaderMagicNumber(magicNumber, bitness);

        deserializer.SkipByte(); // MajorLinkerVersion
        deserializer.SkipByte(); // MinorLinkerVersion
        deserializer.SkipUInt();; // SizeOfCode
        deserializer.SkipUInt(); // SizeOfInitializedData
        deserializer.SkipUInt(); // SizeOfUninitializedData
        deserializer.SkipUInt(); // AddressOfEntryPoint
        deserializer.SkipUInt(); // BaseOfCode
        if (bitness == Bitness.Bitness32)
            deserializer.SkipUInt(); // BaseOfData
        deserializer.SkipPointer(bitness); // ImageBase
        deserializer.SkipUInt(); // SectionAlignment
        deserializer.SkipUInt(); // FileAlignment
        deserializer.SkipUShort(); // MajorOperatingSystemVersion
        deserializer.SkipUShort(); // MinorOperatingSystemVersion
        deserializer.SkipUShort(); // MajorImageVersion
        deserializer.SkipUShort(); // MinorImageVersion
        deserializer.SkipUShort(); // MajorSubsystemVersion
        deserializer.SkipUShort(); // MinorSubsystemVersion
        deserializer.SkipUInt(); // Win32VersionValue
        deserializer.SkipUInt(); // SizeOfImage
        deserializer.SkipUInt(); // SizeOfHeaders
        deserializer.SkipUInt(); // CheckSum
        deserializer.SkipUShort(); // Subsystem
        deserializer.SkipUShort(); // DllCharacteristics
        deserializer.SkipPointer(bitness); // SizeOfStackReserve
        deserializer.SkipPointer(bitness); // SizeOfStackCommit
        deserializer.SkipPointer(bitness); // SizeOfHeapReserve
        deserializer.SkipPointer(bitness); // SizeOfHeapCommit
        deserializer.SkipUInt(); // LoaderFlags
        deserializer.SkipUInt(); // NumberOfRvaAndSizes

        const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
        const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;
        const int SizeOfDataDirectory = sizeof(uint) + sizeof(uint);
        
        deserializer.SkipBytes(SizeOfDataDirectory * (IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR));
        var dotnetHeader = await ReadDataDirectoryAsync(deserializer);
        deserializer.SkipBytes(SizeOfDataDirectory * (IMAGE_NUMBEROF_DIRECTORY_ENTRIES - IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR - 1));

        return dotnetHeader.VirtualAddress != 0;
    }

    private static void EnsureCorrectImageHeaderMagicNumber(uint magic)
    {
        const uint PE00 = 0x00004550;
        if (magic != PE00)
            throw new ParsingException("Wrong NT Header magic number");
    }
    
    private static void EnsureCorrectOptionalHeaderMagicNumber(ushort magic, Bitness bitness)
    {
        const ushort IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        const ushort IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;
        const ushort IMAGE_ROM_OPTIONAL_HDR_MAGIC = 0x107;

        switch (magic)
        {
            case IMAGE_NT_OPTIONAL_HDR32_MAGIC:
                if (bitness != Bitness.Bitness32)
                    throw new ParsingException($"Found 32 bit header in {bitness} binary.");
                break;
            case IMAGE_NT_OPTIONAL_HDR64_MAGIC:
                if (bitness != Bitness.Bitness64)
                    throw new ParsingException($"Found 64 bit header in {bitness} binary.");
                break;
            default:
                throw new ParsingException("Unknown optional PE Header magic number.");
        }
    }

    private static readonly byte[] DosMagicNumbers = {0x4D, 0x5A};

    private static bool CheckDosHeaderMagicNumber(byte[] magicNumbers) => DosMagicNumbers.SequenceEqual(magicNumbers);

    private async Task<(uint VirtualAddress, uint Size)> ReadDataDirectoryAsync(Deserializer deserializer) 
        => (await deserializer.ReadUInt(), await deserializer.ReadUInt());

    private static (Architecture, Bitness) ParseArchitecture(uint i)
    {
        const ushort IMAGE_FILE_MACHINE_I386 = 0x014c;
        const ushort IMAGE_FILE_MACHINE_IA64 = 0x0200;
        const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        const ushort IMAGE_FILE_MACHINE_ARM64 = 0xAA64;

        var bitness = (i & 0x00FF) == 0x64 ? Bitness.Bitness64: Bitness.Bitness32;

        return (GetArchitecture(), bitness);
        
        Architecture GetArchitecture() => i switch
        {
            IMAGE_FILE_MACHINE_I386 => Architecture.i386,
            IMAGE_FILE_MACHINE_AMD64 => Architecture.Amd64,
            IMAGE_FILE_MACHINE_IA64 => Architecture.ia64,
            IMAGE_FILE_MACHINE_ARM64 => Architecture.Arm64,
            _ => Architecture.Unknown
        };
    }

    private static async Task<DosHeaderInfo?> TryParseHeader(Deserializer deserializer)
    {
        var magicNumbers = await deserializer.ReadBytesAsync(2);

        if (!CheckDosHeaderMagicNumber(magicNumbers))
            return null;
        
        deserializer.SkipUShort(); // e_cblp
        deserializer.SkipUShort(); // e_cp
        deserializer.SkipUShort(); // e_crlc
        deserializer.SkipUShort(); // e_cparhdr
        deserializer.SkipUShort(); // e_minalloc
        deserializer.SkipUShort(); // e_maxalloc
        deserializer.SkipUShort(); // e_ss
        deserializer.SkipUShort(); // e_sp
        deserializer.SkipUShort(); // e_csum
        deserializer.SkipUShort(); // e_ip
        deserializer.SkipUShort(); // e_cs
        deserializer.SkipUShort(); // e_lfarlc
        deserializer.SkipUShort(); // e_ovno
        deserializer.SkipBytes(sizeof(ushort) * 4); // e_res
        deserializer.SkipUShort(); // e_oemid
        deserializer.SkipUShort(); // e_oeminfo
        deserializer.SkipBytes(sizeof(ushort) * 10); // e_res2
        var exeOffset = await deserializer.ReadUInt();

        return new DosHeaderInfo {ExeOffset = exeOffset};
    }

    private struct DosHeaderInfo
    {
        public uint ExeOffset { get; init; }
    }
}