using System.Collections.Immutable;
using FormatParser.Helpers;

namespace FormatParser.MachO;

public class MachODecoder : IBinaryFormatDecoder
{
    private static readonly ImmutableDictionary<byte[], (Bitness bitness, Endianness Endianess, bool IsFat)> MagicNumbersNonFat =
            new Dictionary<byte[], (Bitness bitness, Endianness Endianess, bool IsFat)>(ArrayComparer<byte>.Instance)
                {
                    { new byte[] { 0xFE, 0xED, 0xFA, 0xCE }, (Bitness.Bitness32, Endianness.BigEndian, false) },
                    { new byte[] { 0xCE, 0xFA, 0xED, 0xFE }, (Bitness.Bitness32, Endianness.LittleEndian, false) },
                    { new byte[] { 0xFE, 0xED, 0xFA, 0xCF }, (Bitness.Bitness64, Endianness.BigEndian, false) },
                    { new byte[] { 0xCF, 0xFA, 0xED, 0xFE }, (Bitness.Bitness64, Endianness.LittleEndian, false) },
                }.ToImmutableDictionary(ArrayComparer<byte>.Instance);

    private static readonly ImmutableDictionary<byte[], (Bitness bitness, Endianness Endianess, bool IsFat)>
        MagicNumbers = MagicNumbersNonFat
            .ToImmutableDictionary(ArrayComparer<byte>.Instance)
                .Add(new byte[] { 0xCA, 0xFE, 0xBA, 0xBE }, (Bitness.Bitness32, Endianness.BigEndian, true))
                .Add(new byte[] { 0xBE, 0xBA, 0xFE, 0xCA }, (Bitness.Bitness32, Endianness.LittleEndian, true))
                .Add(new byte[] { 0xCA, 0xFE, 0xBA, 0xBF }, (Bitness.Bitness64, Endianness.BigEndian, true))
                .Add(new byte[] { 0xBF, 0xBA, 0xFE, 0xCA }, (Bitness.Bitness64, Endianness.LittleEndian, true));

    public async Task<IFileFormatInfo?> TryDecodeAsync(StreamingBinaryReader streamingBinaryReader)
    {
        streamingBinaryReader.Offset = 0;

        if (streamingBinaryReader.Length < 4)
            return null;
        
        var header = await streamingBinaryReader.ReadBytesAsync(4);
        
        if (!MagicNumbers.TryGetValue(header, out var tuple))
            throw new Exception("Not a Mach O file.");

        var (bitness, endianness, isFat) = tuple;
        streamingBinaryReader.SetEndianess(endianness);

        if (!isFat)
            return await ReadNonFatHeader(streamingBinaryReader, bitness, endianness);

        return await ReadFatFormatInfo(streamingBinaryReader, endianness, bitness);
    }

    private static async Task<FatMachOFileFormatInfo> ReadFatFormatInfo(StreamingBinaryReader streamingBinaryReader, Endianness endianness, Bitness bitness)
    {
        var numberOfArchitectures = (int)await streamingBinaryReader.ReadUInt();

        var headers = new List<(Architecture, ulong Offset)>(numberOfArchitectures);
        
        for (int i = 0; i < numberOfArchitectures; i++)
            headers.Add(await ReadFatArchs(streamingBinaryReader, bitness));

        var result = new List<MachOFileFormatInfo>(numberOfArchitectures);
        foreach (var (architecture, offset) in headers)
        {
            streamingBinaryReader.Offset = (long )offset;
            var header = await streamingBinaryReader.ReadBytesAsync(4);

            (bitness, endianness, _) = MagicNumbersNonFat[header];
            
            streamingBinaryReader.SetEndianess(endianness);
            streamingBinaryReader.Offset = (long) offset;
            
            result.Add(await ReadNonFatHeader(streamingBinaryReader, bitness, endianness));
        }

        return new FatMachOFileFormatInfo(endianness, bitness, result.ToImmutableArray());
    }

    private static async Task<MachOFileFormatInfo> ReadNonFatHeader(StreamingBinaryReader streamingBinaryReader, Bitness bitness, Endianness endianness)
    {
        var (numberOfCommands, architecture) = await ReadNotFatHeaderAsync(streamingBinaryReader, bitness);
        const uint LC_CODE_SIGNATURE = 0x1d;
        
        for (int i = 0; i < numberOfCommands; i++)
        {
            var command = await streamingBinaryReader.ReadUInt();
            var commandSize = await streamingBinaryReader.ReadUInt();

            if (command != LC_CODE_SIGNATURE)
                streamingBinaryReader.SkipBytes(commandSize);
            else
                return new MachOFileFormatInfo(endianness, bitness, architecture, true);
        }
        
        return new MachOFileFormatInfo(endianness, bitness, architecture, false);
    }

    private static async Task<(Architecture, ulong offset)> ReadFatArchs(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        var architecture = ParseCPUType(await streamingBinaryReader.ReadInt()); // cputype
        streamingBinaryReader.SkipInt(); // cpusubtype
        var offset = await streamingBinaryReader.ReadPointer(bitness); // offset
        streamingBinaryReader.SkipPointer(bitness); // size
        streamingBinaryReader.SkipPointer(bitness); // align
        if (bitness == Bitness.Bitness64)
            streamingBinaryReader.SkipUInt(); // reserved
        
        return (architecture, (ulong)offset);
    }
    
    private static async Task<(uint NumberOfCommands, Architecture Architecture)> ReadNotFatHeaderAsync(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        var architecture = ParseCPUType(await streamingBinaryReader.ReadInt()); // cputype
        streamingBinaryReader.SkipInt(); // cpusubtype
        streamingBinaryReader.SkipUInt(); // filetype
        var numberOfCommands = await streamingBinaryReader.ReadUInt(); // ncmds
        streamingBinaryReader.SkipUInt(); // sizeofcmds
        streamingBinaryReader.SkipUInt(); // flags
        
        if (bitness == Bitness.Bitness64)
            streamingBinaryReader.SkipUInt(); // reserved

        return (numberOfCommands, architecture);
    }

    private static Architecture ParseCPUType(int type)
    {
        const int CPU_TYPE_I386 = 7;
        const int CPU_ARCH_ABI64 = 0x1000000;
        const int CPU_TYPE_X86_64 = CPU_TYPE_I386 | CPU_ARCH_ABI64;

        return type switch
        {
            CPU_TYPE_I386 => Architecture.i386,
            CPU_TYPE_X86_64 => Architecture.Amd64,
            _ => Architecture.Unknown
        };
    }
}
