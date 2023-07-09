using FormatParser.Helpers;

namespace FormatParser.ELF;

public class ElfDecoder : IBinaryFormatDecoder
{
    private static byte[] ElfMagicBytes = {0x7F, (byte)'E', (byte)'L', (byte)'F'};

    public async Task<IFileFormatInfo?> TryDecodeAsync(StreamingBinaryReader streamingBinaryReader)
    {
        streamingBinaryReader.Offset = 0;

        var elfHeader = await TryReadElfHeader(streamingBinaryReader);

        if (elfHeader == null)
            return null;

        var (endianness, programHeadersNumber, bitness, architecture) = elfHeader.Value;

        const int PT_INTERP = 3;
        for (var i = 0; i < programHeadersNumber; i++)
        {
            var (type, offset, size) = await ReadProgramHeader(streamingBinaryReader, bitness);
            if (PT_INTERP == type)
            {
                streamingBinaryReader.Offset = (long)offset;
                var interpreter = await streamingBinaryReader.ReadNulTerminatingStringAsync((int) size);
                return new ElfFileFormatInfo(endianness, bitness, architecture , interpreter);
            }
        }

        return new ElfFileFormatInfo(endianness, bitness, architecture, null);
    }

    private static async Task<ElfHeaderInfo?> TryReadElfHeader(StreamingBinaryReader streamingBinaryReader)
    {
        var header = await streamingBinaryReader.TryReadArraySegment(16);
        if (header.Count < 16)
            return null;

        if (!ElfMagicBytes.SequenceEqual(header.GetSubSegment(4)))
            return null;

        var bitness = ParseBitness(header[4]);
        var endianness = ParseEndianness(header[5]);
        streamingBinaryReader.SetEndianness(endianness);
        
        streamingBinaryReader.SkipUShort(); // e_type
        var architecture = ParseArhitecture (await streamingBinaryReader.ReadUShort()); // e_machine
        streamingBinaryReader.SkipUInt(); // e_version
        streamingBinaryReader.SkipPointer(bitness); // e_entry
        streamingBinaryReader.SkipPointer(bitness); // e_phoff
        streamingBinaryReader.SkipPointer(bitness); // e_shoff
        streamingBinaryReader.SkipUInt(); // e_flags
        streamingBinaryReader.SkipUShort(); // e_ehsize
        streamingBinaryReader.SkipUShort(); // e_phentsize
        var programHeadersNumber = await streamingBinaryReader.ReadUShort(); // e_phnum
        streamingBinaryReader.SkipUShort(); // e_shentsize
        streamingBinaryReader.SkipUShort(); // e_shnum
        streamingBinaryReader.SkipUShort(); // e_shstrndx

        return new ElfHeaderInfo {ProgramHeadersNumber = programHeadersNumber, Bitness = bitness, Architecture = architecture, Endianness = endianness};
    }

    private static Architecture ParseArhitecture(ushort architecture)
    {
        const ushort EM_X86_64 = 62;

        if (architecture == EM_X86_64)
            return Architecture.Amd64;

        return Architecture.Unknown;
    }

    private static async Task<(uint Type, ulong Offset, ulong size)> ReadProgramHeader(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        if (bitness == Bitness.Bitness32)
        {
            var type = await streamingBinaryReader.ReadUInt(); // p_type
            var offset = await streamingBinaryReader.ReadUInt(); // p_offset
            streamingBinaryReader.SkipUInt(); // p_vaddr
            await streamingBinaryReader.ReadUInt(); // p_paddr
            var size = await streamingBinaryReader.ReadUInt(); // p_filesz
            streamingBinaryReader.SkipUInt(); // p_memsz
            streamingBinaryReader.SkipUInt(); // p_flags
            streamingBinaryReader.SkipUInt(); // p_align
            
            return (type, offset, size);
        }

        if (bitness == Bitness.Bitness64)
        {
            var type = await streamingBinaryReader.ReadUInt(); // p_type
            streamingBinaryReader.SkipUInt(); // p_flags
            var offset = await streamingBinaryReader.ReadULong(); // p_offset
            streamingBinaryReader.SkipUlong(); // p_vaddr
            streamingBinaryReader.SkipUlong(); // p_paddr
            var size = await streamingBinaryReader.ReadULong(); // p_filesz
            streamingBinaryReader.SkipUlong(); // p_memsz
            streamingBinaryReader.SkipUlong(); // p_align
            
            return (type, offset, size);
        }

        throw new Exception();
    }

    private static Bitness ParseBitness(byte b)
    {
        const byte ELFCLASS32 = 1;
        const byte ELFCLASS64 = 2;

        return b switch
        {
            ELFCLASS32 => Bitness.Bitness32,
            ELFCLASS64 => Bitness.Bitness64,
            _ => throw new Exception("Wrong byte at bitness position.")
        };
    }

    private static Endianness ParseEndianness(byte b)
    {
        const byte ELFDATA2LSB = 1;
        const byte ELFDATA2MSB = 2;

        return b switch
        {
            ELFDATA2LSB => Endianness.LittleEndian,
            ELFDATA2MSB => Endianness.BigEndian,
            _ => throw new Exception("Wrong byte at endianness position.")
        };
    }

    private record struct ElfHeaderInfo(Endianness Endianness, ushort ProgramHeadersNumber, Bitness Bitness, Architecture Architecture);
}