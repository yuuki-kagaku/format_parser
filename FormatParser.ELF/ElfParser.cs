namespace FormatParser.ELF;

public class ElfParser {
    private static byte[] ElfMagicBytes = {0x7F, (byte)'E', (byte)'L', (byte)'F'};

    public static async Task<ParsingResult<ElfData>> DeserializeAsync(Stream stream)
    {
        var deserializer = new Deserializer(stream);
        var header = await deserializer.ReadBytes(16);

        if (!ElfMagicBytes.SequenceEqual(new ArraySegment<byte>(header, 0, 4)))
            return ParsingResult.Error<ElfData>("Wrong ELF magic bytes");

        var bitness = ParseBitness(header[4]);
        var endianess = ParseEndianess(header[5]);
        deserializer.SetEndianess(endianess);
        
        deserializer.SkipShort(); // e_type
        var architecture = ParseArhitecture (await deserializer.ReadUShort()); // e_machine
        deserializer.SkipInt(); // e_version
        deserializer.SkipPointer(bitness); // e_entry
        deserializer.SkipPointer(bitness); // e_phoff
        deserializer.SkipPointer(bitness); // e_shoff
        deserializer.SkipInt(); // e_flags
        deserializer.SkipShort(); // e_ehsize
        deserializer.SkipShort(); // e_phentsize
        var programHeadersNumber = await deserializer.ReadShort(); // e_phnum
        deserializer.SkipShort(); // e_shentsize
        deserializer.SkipShort(); // e_shnum
        deserializer.SkipShort(); // e_shstrndx

        const int PT_INTERP = 3;
        for (var i = 0; i < programHeadersNumber; i++)
        {
            var (type, offset, size) = await ReadPhdr(deserializer, bitness);
            if (PT_INTERP == type)
            {
                deserializer.Offset = offset;
                var interpreter = await deserializer.ReadNulTerminatingStringAsync((int) size);
                return new ParsingResult<ElfData>(new ElfData(endianess, bitness, architecture , interpreter), null);
            }
        }

        return new ParsingResult<ElfData>(new ElfData(endianess, bitness, architecture, null), null);
    }

    private static Architecture ParseArhitecture(ushort architecture)
    {
        const ushort EM_X86_64 = 62;

        if (architecture == EM_X86_64)
            return Architecture.Amd64;

        return Architecture.Unknown;
    }

    private static async Task<(int Type, long Offset, long size)> ReadPhdr(Deserializer deserializer, Bitness bitness)
    {
        if (bitness == Bitness.Bitness32)
        {
            var type = await deserializer.ReadInt(); // p_type
            var offset = await deserializer.ReadInt(); // p_offset
            await deserializer.ReadInt(); // p_vaddr
            await deserializer.ReadInt(); // p_paddr
            var p_filesz = await deserializer.ReadInt(); // p_filesz
            await deserializer.ReadInt(); // p_memsz
            await deserializer.ReadInt(); // p_flags
            await deserializer.ReadInt(); // p_align
            
            return (type, offset, p_filesz);
        }

        if (bitness == Bitness.Bitness64)
        {
            var type = await deserializer.ReadInt(); // p_type
            await deserializer.ReadInt(); // p_flags
            var offset = await deserializer.ReadLong(); // p_offset
            await deserializer.ReadLong(); // p_vaddr
            await deserializer.ReadLong(); // p_paddr
            var p_filesz = await deserializer.ReadLong(); // p_filesz
            await deserializer.ReadLong(); // p_memsz
            await deserializer.ReadLong(); // p_align
            
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

    private static Endianess ParseEndianess(byte b)
    {
        const byte ELFDATA2LSB = 1;
        const byte ELFDATA2MSB = 2;

        if (b == ELFDATA2LSB)
            return Endianess.LittleEndian;

        if (b == ELFDATA2MSB)
            return Endianess.BigEndian;

        return Endianess.Unknown;
    }
}