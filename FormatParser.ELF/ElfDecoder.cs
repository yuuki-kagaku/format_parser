using FormatParser.Helpers;

namespace FormatParser.ELF;

public class ElfDecoder : IBinaryFormatDecoder
{
    private static byte[] ElfMagicBytes = {0x7F, (byte)'E', (byte)'L', (byte)'F'};

    public async Task<IFileFormatInfo?> TryDecodeAsync(StreamingBinaryReader streamingBinaryReader)
    {
        streamingBinaryReader.Offset = 0;
        var header = await streamingBinaryReader.TryReadArraySegment(16);
        if (header.Count < 16)
            return null;

        if (!ElfMagicBytes.SequenceEqual(header.GetSubSegment(4)))
            return null;

        var bitness = ParseBitness(header[4]);
        var endianness = ParseEndianness(header[5]);
        streamingBinaryReader.SetEndianess(endianness);
        
        streamingBinaryReader.SkipShort(); // e_type
        var architecture = ParseArhitecture (await streamingBinaryReader.ReadUShort()); // e_machine
        streamingBinaryReader.SkipInt(); // e_version
        streamingBinaryReader.SkipPointer(bitness); // e_entry
        streamingBinaryReader.SkipPointer(bitness); // e_phoff
        streamingBinaryReader.SkipPointer(bitness); // e_shoff
        streamingBinaryReader.SkipInt(); // e_flags
        streamingBinaryReader.SkipShort(); // e_ehsize
        streamingBinaryReader.SkipShort(); // e_phentsize
        var programHeadersNumber = await streamingBinaryReader.ReadShort(); // e_phnum
        streamingBinaryReader.SkipShort(); // e_shentsize
        streamingBinaryReader.SkipShort(); // e_shnum
        streamingBinaryReader.SkipShort(); // e_shstrndx

        const int PT_INTERP = 3;
        for (var i = 0; i < programHeadersNumber; i++)
        {
            var (type, offset, size) = await ReadPhdr(streamingBinaryReader, bitness);
            if (PT_INTERP == type)
            {
                streamingBinaryReader.Offset = offset;
                var interpreter = await streamingBinaryReader.ReadNulTerminatingStringAsync((int) size);
                return new ElfFileFormatInfo(endianness, bitness, architecture , interpreter);
            }
        }

        return new ElfFileFormatInfo(endianness, bitness, architecture, null);
    }

    private static Architecture ParseArhitecture(ushort architecture)
    {
        const ushort EM_X86_64 = 62;

        if (architecture == EM_X86_64)
            return Architecture.Amd64;

        return Architecture.Unknown;
    }

    private static async Task<(int Type, long Offset, long size)> ReadPhdr(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        if (bitness == Bitness.Bitness32)
        {
            var type = await streamingBinaryReader.ReadInt(); // p_type
            var offset = await streamingBinaryReader.ReadInt(); // p_offset
            await streamingBinaryReader.ReadInt(); // p_vaddr
            await streamingBinaryReader.ReadInt(); // p_paddr
            var p_filesz = await streamingBinaryReader.ReadInt(); // p_filesz
            await streamingBinaryReader.ReadInt(); // p_memsz
            await streamingBinaryReader.ReadInt(); // p_flags
            await streamingBinaryReader.ReadInt(); // p_align
            
            return (type, offset, p_filesz);
        }

        if (bitness == Bitness.Bitness64)
        {
            var type = await streamingBinaryReader.ReadInt(); // p_type
            await streamingBinaryReader.ReadInt(); // p_flags
            var offset = await streamingBinaryReader.ReadLong(); // p_offset
            await streamingBinaryReader.ReadLong(); // p_vaddr
            await streamingBinaryReader.ReadLong(); // p_paddr
            var p_filesz = await streamingBinaryReader.ReadLong(); // p_filesz
            await streamingBinaryReader.ReadLong(); // p_memsz
            await streamingBinaryReader.ReadLong(); // p_align
            
            return (type, offset, p_filesz);
        }

        throw new Exception();
    }

    private static Bitness ParseBitness(byte b)
    {
        const byte ELFCLASS32 = 1;
        const byte ELFCLASS64 = 2;

        if (b == ELFCLASS32)
            return Bitness.Bitness32;

        if (b == ELFCLASS64)
            return Bitness.Bitness64;

        return Bitness.Unknown;
    }

    private static Endianness ParseEndianness(byte b)
    {
        const byte ELFDATA2LSB = 1;
        const byte ELFDATA2MSB = 2;

        if (b == ELFDATA2LSB)
            return Endianness.LittleEndian;

        if (b == ELFDATA2MSB)
            return Endianness.BigEndian;

        return Endianness.Unknown;
    }
}