using FormatParser.Domain;
using FormatParser.Helpers.BinaryReader;

namespace FormatParser.PE;

public class PeDetector : IBinaryFormatDetector
{
    public async Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader)
    {
        binaryReader.SetEndianness(Endianness.LittleEndian);
        var dosHeader = await TryReadDosHeaderAsync(binaryReader);

        if (dosHeader == null)
            return null;

        if (dosHeader.Value.ExeOffset == 0)
            return new DosMzFileFormatInfo();

        binaryReader.Offset = dosHeader.Value.ExeOffset;

        var (architecture, bitness, sizeOfOptionalHeader) = await ReadImageFileHeaderAsync(binaryReader);
        var isDotNet = await ReadOptionalHeaderAsync(binaryReader, sizeOfOptionalHeader, bitness);

        return new PeFileFormatInfo(bitness, architecture, isDotNet);
    }

    private static async Task<(Architecture, Bitness, ushort SizeOfOptionalHeader)> ReadImageFileHeaderAsync(StreamingBinaryReader streamingBinaryReader)
    {
        EnsureCorrectImageHeaderMagicNumber(await streamingBinaryReader.ReadUIntAsync()); // Magic
        var (architecture, bitness) = PEArchitectureConverter.Convert(await streamingBinaryReader.ReadUShortAsync()); // Machine
        streamingBinaryReader.SkipUShort(); // NumberOfSections
        streamingBinaryReader.SkipUInt(); //  TimeDateStamp
        streamingBinaryReader.SkipUInt(); //  PointerToSymbolTable
        streamingBinaryReader.SkipUInt(); //  NumberOfSymbols
        var sizeOfOptionalHeader = await streamingBinaryReader.ReadUShortAsync(); // SizeOfOptionalHeader
        streamingBinaryReader.SkipUShort(); // Characteristics

        return (architecture, bitness, sizeOfOptionalHeader);
    }

    private async Task<bool> ReadOptionalHeaderAsync(StreamingBinaryReader streamingBinaryReader, ushort size, Bitness bitness)
    {
        if (size < PEConstants.OptionalHeaderMinSuze)
            return false;
        
        var magicNumber = await streamingBinaryReader.ReadUShortAsync(); // Magic
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
        
        streamingBinaryReader.SkipBytes(PEConstants.SizeOfDataDirectory * (PEConstants.IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR));
        var dotnetHeader = await ReadDataDirectoryAsync(streamingBinaryReader);
        streamingBinaryReader.SkipBytes(PEConstants.SizeOfDataDirectory * (PEConstants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES - PEConstants.IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR - 1));

        return dotnetHeader.VirtualAddress != 0;
    }

    private static void EnsureCorrectImageHeaderMagicNumber(uint magic)
    {
        if (magic != PEConstants.ImageHeaderMagicNumber)
            throw new FormatParserException("Wrong NT Header magic number");
    }
    
    private static void EnsureCorrectOptionalHeaderMagicNumber(ushort magic, Bitness bitness)
    {
        switch (magic)
        {
            case PEConstants.IMAGE_NT_OPTIONAL_HDR32_MAGIC:
                if (bitness != Bitness.Bitness32)
                    throw new FormatParserException($"Found 32 bit header in {bitness} binary.");
                break;
            case PEConstants.IMAGE_NT_OPTIONAL_HDR64_MAGIC:
                if (bitness != Bitness.Bitness64)
                    throw new FormatParserException($"Found 64 bit header in {bitness} binary.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(magic), "Unknown optional PE Header magic number.");
        }
    }
    
    private static bool CheckDosHeaderMagicNumber(byte[] magicNumbers) => PEConstants.DosMagicNumbers.SequenceEqual(magicNumbers);

    private async Task<(uint VirtualAddress, uint Size)> ReadDataDirectoryAsync(StreamingBinaryReader streamingBinaryReader) 
        => (await streamingBinaryReader.ReadUIntAsync(), await streamingBinaryReader.ReadUIntAsync());

    private static async Task<DosHeaderInfo?> TryReadDosHeaderAsync(StreamingBinaryReader streamingBinaryReader)
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
        var exeOffset = await streamingBinaryReader.ReadUIntAsync();

        return new DosHeaderInfo {ExeOffset = exeOffset};
    }

    private record struct DosHeaderInfo(uint ExeOffset);
}