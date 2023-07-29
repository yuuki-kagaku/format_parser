using System.Collections.Immutable;
using FormatParser.Domain;
using FormatParser.Helpers.BinaryReader;

namespace FormatParser.MachO;

public class MachODetector : IBinaryFormatDetector
{
    public async Task<IFileFormatInfo?> TryDetectAsync(StreamingBinaryReader binaryReader)
    {
        if (binaryReader.Length < 4)
            return null;
        
        var header = await binaryReader.ReadBytesAsync(4);
        
        if (!MachOMagicNumbers.All.TryGetValue(header, out var tuple))
            return null;

        var (bitness, endianness, isFat) = tuple;

        return isFat
            ? await ReadFatFormatInfoAsync(binaryReader, endianness, bitness)
            : await ReadNonFatFormatInfoAsync(binaryReader, bitness, endianness);
    }

    private static async Task<FatMachOFileFormatInfo> ReadFatFormatInfoAsync(StreamingBinaryReader binaryReader, Endianness endianness, Bitness bitness)
    {
        binaryReader.SetEndianness(endianness);
        var numberOfArchitectures = (int)await binaryReader.ReadUIntAsync();

        var headers = new List<(Architecture, ulong Offset)>(numberOfArchitectures);
        
        for (var i = 0; i < numberOfArchitectures; i++)
            headers.Add(await ReadArchitecturesOfFatFileAsync(binaryReader, bitness));

        var result = new List<MachOFileFormatInfo>(numberOfArchitectures);
        foreach (var (architecture, offset) in headers)
        {
            binaryReader.Offset = (long) offset;
            var header = await binaryReader.ReadBytesAsync(4);

            (bitness, endianness, _) = MachOMagicNumbers.NonFat[header];
            
            binaryReader.SetEndianness(endianness);
            
            result.Add(await ReadNonFatFormatInfoAsync(binaryReader, bitness, endianness));
        }

        return new FatMachOFileFormatInfo(endianness, bitness, result.ToImmutableArray());
    }

    private static async Task<MachOFileFormatInfo> ReadNonFatFormatInfoAsync(StreamingBinaryReader binaryReader, Bitness bitness, Endianness endianness)
    {
        binaryReader.SetEndianness(endianness);
        var (numberOfCommands, architecture) = await ReadNonFatHeaderAsync(binaryReader, bitness);

        for (var i = 0; i < numberOfCommands; i++)
        {
            var commandType = await binaryReader.ReadUIntAsync();
            var commandSize = await binaryReader.ReadUIntAsync();

            if (commandType != MachOConstants.LC_CODE_SIGNATURE)
                binaryReader.SkipBytes(commandSize - 2 * sizeof(uint));
            else
                return new MachOFileFormatInfo(endianness, bitness, architecture, true);
        }
        
        return new MachOFileFormatInfo(endianness, bitness, architecture, false);
    }

    private static async Task<(Architecture, ulong Offset)> ReadArchitecturesOfFatFileAsync(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        var architecture = MachOArchitectureConverter.Convert(await streamingBinaryReader.ReadIntAsync()); // cputype
        streamingBinaryReader.SkipInt(); // cpusubtype
        var offset = await streamingBinaryReader.ReadPointerAsync(bitness); // offset
        streamingBinaryReader.SkipPointer(bitness); // size
        streamingBinaryReader.SkipPointer(bitness); // align
        
        if (bitness == Bitness.Bitness64)
            streamingBinaryReader.SkipUInt(); // reserved
        
        return (architecture, offset);
    }
    
    private static async Task<NonFatHeader> ReadNonFatHeaderAsync(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        var architecture = MachOArchitectureConverter.Convert(await streamingBinaryReader.ReadIntAsync()); // cputype
        streamingBinaryReader.SkipInt(); // cpusubtype
        streamingBinaryReader.SkipUInt(); // filetype
        var numberOfCommands = await streamingBinaryReader.ReadUIntAsync(); // ncmds
        streamingBinaryReader.SkipUInt(); // sizeofcmds
        streamingBinaryReader.SkipUInt(); // flags
        
        if (bitness == Bitness.Bitness64)
            streamingBinaryReader.SkipUInt(); // reserved

        return new (numberOfCommands, architecture);
    }

    private record struct NonFatHeader(uint NumberOfCommands, Architecture Architecture);
}
