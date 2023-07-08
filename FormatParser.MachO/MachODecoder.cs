using System.Collections.Immutable;

namespace FormatParser.MachO;

public class MachODecoder : IBinaryFormatDecoder
{
    private static readonly ImmutableDictionary<byte[], (Bitness bitness, Endianess Endianess, bool IsFat)> MagicNumbersNonFat =
            new Dictionary<byte[], (Bitness bitness, Endianess Endianess, bool IsFat)>(ArrayComparer<byte>.Instance)
                {
                    { new byte[] { 0xFE, 0xED, 0xFA, 0xCE }, (Bitness.Bitness32, Endianess.BigEndian, false) },
                    { new byte[] { 0xCE, 0xFA, 0xED, 0xFE }, (Bitness.Bitness32, Endianess.LittleEndian, false) },
                    { new byte[] { 0xFE, 0xED, 0xFA, 0xCF }, (Bitness.Bitness64, Endianess.BigEndian, false) },
                    { new byte[] { 0xCF, 0xFA, 0xED, 0xFE }, (Bitness.Bitness64, Endianess.LittleEndian, false) },
                }.ToImmutableDictionary(ArrayComparer<byte>.Instance);

    private static readonly ImmutableDictionary<byte[], (Bitness bitness, Endianess Endianess, bool IsFat)>
        MagicNumbers = MagicNumbersNonFat
            .ToImmutableDictionary(ArrayComparer<byte>.Instance)
                .Add(new byte[] { 0xCA, 0xFE, 0xBA, 0xBE }, (Bitness.Bitness32, Endianess.BigEndian, true))
                .Add(new byte[] { 0xBE, 0xBA, 0xFE, 0xCA }, (Bitness.Bitness32, Endianess.LittleEndian, true))
                .Add(new byte[] { 0xCA, 0xFE, 0xBA, 0xBF }, (Bitness.Bitness64, Endianess.BigEndian, true))
                .Add(new byte[] { 0xBF, 0xBA, 0xFE, 0xCA }, (Bitness.Bitness64, Endianess.LittleEndian, true));

    public async Task<IData?> TryDecodeAsync(Deserializer deserializer)
    {
        deserializer.Offset = 0;
        var header = await deserializer.ReadBytes(4);
        
        if (!MagicNumbers.TryGetValue(header, out var tuple))
            throw new Exception("Not a Mach O file.");

        var (bitness, endianness, isFat) = tuple;
        deserializer.SetEndianess(endianness);

        if (!isFat)
            return await ReadNonFatHeader(deserializer, bitness, endianness);

        var fatMachOData = new FatMachOData(endianness, bitness, ImmutableArray<MachOData>.Empty);
        var numberOfArchitectures = (int)await deserializer.ReadUInt();

        var headers = new List<(Architecture, ulong Offset)>(numberOfArchitectures);
        for (int i = 0; i < numberOfArchitectures; i++)
            headers.Add(await ReadFatArchs(deserializer, bitness));

        var result = new List<MachOData>(numberOfArchitectures);
        foreach (var (architecture, offset) in headers)
        {
            deserializer.Offset = (long )offset;
            header = await deserializer.ReadBytes(4);

            (bitness, endianness, _) = MagicNumbersNonFat[header];
            
            deserializer.SetEndianess(endianness);
            deserializer.Offset = (long) offset;
            
            result.Add(await ReadNonFatHeader(deserializer, bitness, endianness));
        }

        return fatMachOData with { Datas = result.ToImmutableArray() };
    }

    private static async Task<MachOData> ReadNonFatHeader(Deserializer deserializer, Bitness bitness, Endianess endianness)
    {
        var (numberOfCommands, architecture) = await ReadNotFatHeaderAsync(deserializer, bitness);
        const uint LC_CODE_SIGNATURE = 0x1d;
        for (int i = 0; i < numberOfCommands; i++)
        {
            var command = await deserializer.ReadUInt();
            var commandSize = await deserializer.ReadUInt();

            if (command != LC_CODE_SIGNATURE)
                deserializer.SkipBytes(commandSize);
            else
                return new MachOData(endianness, bitness, architecture, true);
        }
        
        return new MachOData(endianness, bitness, architecture, false);
    }

    private static async Task<(Architecture, ulong offset)> ReadFatArchs(Deserializer deserializer, Bitness bitness)
    {
        var architecture = ParseCPUType(await deserializer.ReadInt()); // cputype
        deserializer.SkipInt(); // cpusubtype
        var offset = await deserializer.ReadPointer(bitness); // offset
        deserializer.SkipPointer(bitness); // size
        deserializer.SkipPointer(bitness); // align
        if (bitness == Bitness.Bitness64)
            deserializer.SkipUInt(); // reserved
        
        return (architecture, (ulong)offset);
    }
    
    private static async Task<(uint NumberOfCommands, Architecture Architecture)> ReadNotFatHeaderAsync(Deserializer deserializer, Bitness bitness)
    {
        var architecture = ParseCPUType(await deserializer.ReadInt()); // cputype
        deserializer.SkipInt(); // cpusubtype
        deserializer.SkipUInt(); // filetype
        var numberOfCommands = await deserializer.ReadUInt(); // ncmds
        deserializer.SkipUInt(); // sizeofcmds
        deserializer.SkipUInt(); // flags
        
        if (bitness == Bitness.Bitness64)
            deserializer.SkipUInt(); // reserved

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

public class ArrayComparer<T> : IEqualityComparer<T[]> where T : IEquatable<T>
{
    private static readonly IEqualityComparer<T> elementComparer = EqualityComparer<T>.Default;
    public static readonly ArrayComparer<T> Instance = new ();
    
    public bool Equals(T[]? x, T[]? y)
    {
        if (x == null || y == null)
            return ReferenceEquals(x, y);

        return x.SequenceEqual(y);
    }

    public int GetHashCode(T[] array) => array.Aggregate(array.Length, (current, element) => (current * 443) ^ elementComparer.GetHashCode(element));
}