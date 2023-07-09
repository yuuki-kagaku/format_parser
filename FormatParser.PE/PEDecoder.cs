namespace FormatParser.PE;

public class PEDecoder : IBinaryFormatDecoder
{
    public async Task<IFileFormatInfo?> TryDecodeAsync(StreamingBinaryReader streamingBinaryReader)
    {
        streamingBinaryReader.Offset = 0;
        streamingBinaryReader.SetEndianness(Endianness.LittleEndian);
        var dosHeader = await TryReadHeader(streamingBinaryReader);

        if (dosHeader == null)
            return null;

        streamingBinaryReader.Offset = dosHeader.Value.ExeOffset;

        var (architecture, bitness, sizeOfOptionalHeader) = await ReadImageFileHeaderAsync(streamingBinaryReader);
        var isDotNet = await ReadOptionalHeader(streamingBinaryReader, sizeOfOptionalHeader, bitness);

        return new PeFileFormatInfo(bitness, architecture, isDotNet);
    }

    private static async Task<(Architecture, Bitness, ushort SizeOfOptionalHeader)> ReadImageFileHeaderAsync(StreamingBinaryReader streamingBinaryReader)
    {
        EnsureCorrectImageHeaderMagicNumber(await streamingBinaryReader.ReadUInt()); // Magic
        var (architecture, bitness) = ParseArchitecture(await streamingBinaryReader.ReadUShort()); // Machine
        streamingBinaryReader.SkipUShort(); // NumberOfSections
        streamingBinaryReader.SkipUInt(); //  TimeDateStamp
        streamingBinaryReader.SkipUInt(); //  PointerToSymbolTable
        streamingBinaryReader.SkipUInt(); //  NumberOfSymbols
        var sizeOfOptionalHeader = await streamingBinaryReader.ReadUShort(); // SizeOfOptionalHeader
        streamingBinaryReader.SkipUShort(); // Characteristics

        return (architecture, bitness, sizeOfOptionalHeader);
    }

    private async Task<bool> ReadOptionalHeader(StreamingBinaryReader streamingBinaryReader, ushort size, Bitness bitness)
    {
        if (size < 224)
            return false;
        
        var magicNumber = await streamingBinaryReader.ReadUShort(); // Magic
        EnsureCorrectOptionalHeaderMagicNumber(magicNumber, bitness);

        streamingBinaryReader.SkipByte(); // MajorLinkerVersion
        streamingBinaryReader.SkipByte(); // MinorLinkerVersion
        streamingBinaryReader.SkipUInt();; // SizeOfCode
        streamingBinaryReader.SkipUInt(); // SizeOfInitializedData
        streamingBinaryReader.SkipUInt(); // SizeOfUninitializedData
        streamingBinaryReader.SkipUInt(); // AddressOfEntryPoint
        streamingBinaryReader.SkipUInt(); // BaseOfCode
        if (bitness == Bitness.Bitness32)
            streamingBinaryReader.SkipUInt(); // BaseOfData
        streamingBinaryReader.SkipPointer(bitness); // ImageBase
        streamingBinaryReader.SkipUInt(); // SectionAlignment
        streamingBinaryReader.SkipUInt(); // FileAlignment
        streamingBinaryReader.SkipUShort(); // MajorOperatingSystemVersion
        streamingBinaryReader.SkipUShort(); // MinorOperatingSystemVersion
        streamingBinaryReader.SkipUShort(); // MajorImageVersion
        streamingBinaryReader.SkipUShort(); // MinorImageVersion
        streamingBinaryReader.SkipUShort(); // MajorSubsystemVersion
        streamingBinaryReader.SkipUShort(); // MinorSubsystemVersion
        streamingBinaryReader.SkipUInt(); // Win32VersionValue
        streamingBinaryReader.SkipUInt(); // SizeOfImage
        streamingBinaryReader.SkipUInt(); // SizeOfHeaders
        streamingBinaryReader.SkipUInt(); // CheckSum
        streamingBinaryReader.SkipUShort(); // Subsystem
        streamingBinaryReader.SkipUShort(); // DllCharacteristics
        streamingBinaryReader.SkipPointer(bitness); // SizeOfStackReserve
        streamingBinaryReader.SkipPointer(bitness); // SizeOfStackCommit
        streamingBinaryReader.SkipPointer(bitness); // SizeOfHeapReserve
        streamingBinaryReader.SkipPointer(bitness); // SizeOfHeapCommit
        streamingBinaryReader.SkipUInt(); // LoaderFlags
        streamingBinaryReader.SkipUInt(); // NumberOfRvaAndSizes

        const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
        const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;
        const int SizeOfDataDirectory = sizeof(uint) + sizeof(uint);
        
        streamingBinaryReader.SkipBytes(SizeOfDataDirectory * (IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR));
        var dotnetHeader = await ReadDataDirectoryAsync(streamingBinaryReader);
        streamingBinaryReader.SkipBytes(SizeOfDataDirectory * (IMAGE_NUMBEROF_DIRECTORY_ENTRIES - IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR - 1));

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

    private async Task<(uint VirtualAddress, uint Size)> ReadDataDirectoryAsync(StreamingBinaryReader streamingBinaryReader) 
        => (await streamingBinaryReader.ReadUInt(), await streamingBinaryReader.ReadUInt());

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

    private static async Task<DosHeaderInfo?> TryReadHeader(StreamingBinaryReader streamingBinaryReader)
    {
        if (streamingBinaryReader.Length < 44)
            return null;
        
        var magicNumbers = await streamingBinaryReader.ReadBytesAsync(2);

        if (!CheckDosHeaderMagicNumber(magicNumbers))
            return null;
        
        streamingBinaryReader.SkipUShort(); // e_cblp
        streamingBinaryReader.SkipUShort(); // e_cp
        streamingBinaryReader.SkipUShort(); // e_crlc
        streamingBinaryReader.SkipUShort(); // e_cparhdr
        streamingBinaryReader.SkipUShort(); // e_minalloc
        streamingBinaryReader.SkipUShort(); // e_maxalloc
        streamingBinaryReader.SkipUShort(); // e_ss
        streamingBinaryReader.SkipUShort(); // e_sp
        streamingBinaryReader.SkipUShort(); // e_csum
        streamingBinaryReader.SkipUShort(); // e_ip
        streamingBinaryReader.SkipUShort(); // e_cs
        streamingBinaryReader.SkipUShort(); // e_lfarlc
        streamingBinaryReader.SkipUShort(); // e_ovno
        streamingBinaryReader.SkipBytes(sizeof(ushort) * 4); // e_res
        streamingBinaryReader.SkipUShort(); // e_oemid
        streamingBinaryReader.SkipUShort(); // e_oeminfo
        streamingBinaryReader.SkipBytes(sizeof(ushort) * 10); // e_res2
        var exeOffset = await streamingBinaryReader.ReadUInt();

        return new DosHeaderInfo {ExeOffset = exeOffset};
    }

    private struct DosHeaderInfo
    {
        public uint ExeOffset { get; init; }
    }
}