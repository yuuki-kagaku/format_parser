using FormatParser.Domain;
using FormatParser.Helpers;
using FormatParser.Helpers.BinaryReader;

namespace FormatParser.ELF;

public class ElfDetector : IBinaryFormatDetector
{
    public async Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader)
    {
        var elfHeader = await TryReadElfHeaderAsync(binaryReader);

        if (elfHeader == null)
            return null;

        var (endianness, programHeadersNumber, bitness, architecture) = elfHeader.Value;

        for (var i = 0; i < programHeadersNumber; i++)
        {
            var (type, offset, size) = await ReadProgramHeaderAsync(binaryReader, bitness);
            if (type == ELFConstants.PT_INTERP)
            {
                binaryReader.Offset = (long)offset;
                var interpreter = await binaryReader.ReadNulTerminatingStringAsync((int) size);
                return new ElfFileFormatInfo(endianness, bitness, architecture , interpreter);
            }
        }

        return new ElfFileFormatInfo(endianness, bitness, architecture, null);
    }

    private static async Task<ElfHeaderInfo?> TryReadElfHeaderAsync(StreamingBinaryReader streamingBinaryReader)
    {
        var header = await streamingBinaryReader.TryReadArraySegmentAsync(16);
        if (header.Count < 16)
            return null;

        if (!ELFConstants.ElfMagicBytes.SequenceEqual(header.GetSubSegment(4)))
            return null;

        var bitness = ParseBitness(header[4]);
        var endianness = ParseEndianness(header[5]);
        streamingBinaryReader.SetEndianness(endianness);
        
        streamingBinaryReader.SkipUShort(); // e_type
        var architecture = ElfArchitectureConverter.Convert (await streamingBinaryReader.ReadUShortAsync()); // e_machine
        streamingBinaryReader.SkipUInt(); // e_version
        streamingBinaryReader.SkipPointer(bitness); // e_entry
        streamingBinaryReader.SkipPointer(bitness); // e_phoff
        streamingBinaryReader.SkipPointer(bitness); // e_shoff
        streamingBinaryReader.SkipUInt(); // e_flags
        streamingBinaryReader.SkipUShort(); // e_ehsize
        streamingBinaryReader.SkipUShort(); // e_phentsize
        var programHeadersNumber = await streamingBinaryReader.ReadUShortAsync(); // e_phnum
        streamingBinaryReader.SkipUShort(); // e_shentsize
        streamingBinaryReader.SkipUShort(); // e_shnum
        streamingBinaryReader.SkipUShort(); // e_shstrndx

        return new ElfHeaderInfo {ProgramHeadersNumber = programHeadersNumber, Bitness = bitness, Architecture = architecture, Endianness = endianness};
    }

    private static async Task<ProgramHeaderInfo> ReadProgramHeaderAsync(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        switch (bitness)
        {
            case Bitness.Bitness32:
            {
                var type = await streamingBinaryReader.ReadUIntAsync(); // p_type
                var offset = await streamingBinaryReader.ReadUIntAsync(); // p_offset
                streamingBinaryReader.SkipUInt(); // p_vaddr
                streamingBinaryReader.SkipUInt(); // p_paddr
                var size = await streamingBinaryReader.ReadUIntAsync(); // p_filesz
                streamingBinaryReader.SkipUInt(); // p_memsz
                streamingBinaryReader.SkipUInt(); // p_flags
                streamingBinaryReader.SkipUInt(); // p_align
            
                return new ProgramHeaderInfo(type, offset, size);
            }
            case Bitness.Bitness64:
            {
                var type = await streamingBinaryReader.ReadUIntAsync(); // p_type
                streamingBinaryReader.SkipUInt(); // p_flags
                var offset = await streamingBinaryReader.ReadULongAsync(); // p_offset
                streamingBinaryReader.SkipUlong(); // p_vaddr
                streamingBinaryReader.SkipUlong(); // p_paddr
                var size = await streamingBinaryReader.ReadULongAsync(); // p_filesz
                streamingBinaryReader.SkipUlong(); // p_memsz
                streamingBinaryReader.SkipUlong(); // p_align
            
                return new ProgramHeaderInfo(type, offset, size);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(bitness));
        }
    }

    private static Bitness ParseBitness(byte b) =>
        b switch
        {
            ELFConstants.ELFCLASS32 => Bitness.Bitness32,
            ELFConstants.ELFCLASS64 => Bitness.Bitness64,
            _ => throw new ArgumentOutOfRangeException(nameof(b), "Wrong byte at bitness position.")
        };

    private static Endianness ParseEndianness(byte b) =>
        b switch
        {
            ELFConstants.ELFDATA2LSB => Endianness.LittleEndian,
            ELFConstants.ELFDATA2MSB => Endianness.BigEndian,
            _ => throw new ArgumentOutOfRangeException(nameof(b), "Wrong byte at endianness position.")
        };

    private record struct ElfHeaderInfo(Endianness Endianness, ushort ProgramHeadersNumber, Bitness Bitness, Architecture Architecture);
    private record struct ProgramHeaderInfo(uint Type, ulong Offset, ulong Size);
}