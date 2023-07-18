using System.Collections.Immutable;
using FormatParser.BinaryReader;
using FormatParser.Domain;
using FormatParser.Helpers;

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

        if (isFat)
            return await ReadFatFormatInfo(binaryReader, endianness, bitness);

        return await ReadNonFatFormatInfo(binaryReader, bitness, endianness);
    }

    private static async Task<FatMachOFileFormatInfo> ReadFatFormatInfo(StreamingBinaryReader binaryReader, Endianness endianness, Bitness bitness)
    {
        binaryReader.SetEndianness(endianness);
        var numberOfArchitectures = (int)await binaryReader.ReadUInt();

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
            
            result.Add(await ReadNonFatFormatInfo(binaryReader, bitness, endianness));
        }

        return new FatMachOFileFormatInfo(endianness, bitness, result.ToImmutableArray());
    }

    private static async Task<MachOFileFormatInfo> ReadNonFatFormatInfo(StreamingBinaryReader binaryReader, Bitness bitness, Endianness endianness)
    {
        binaryReader.SetEndianness(endianness);
        var (numberOfCommands, architecture) = await ReadNonFatHeaderAsync(binaryReader, bitness);

        for (var i = 0; i < numberOfCommands; i++)
        {
            var commandType = await binaryReader.ReadUInt();
            var commandSize = await binaryReader.ReadUInt();

            if (commandType != MachOConstants.LC_CODE_SIGNATURE)
                binaryReader.SkipBytes(commandSize - 2 * sizeof(uint));
            else
                return new MachOFileFormatInfo(endianness, bitness, architecture, true);
        }
        
        return new MachOFileFormatInfo(endianness, bitness, architecture, false);
    }

    private static async Task<(Architecture, ulong Offset)> ReadArchitecturesOfFatFileAsync(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        var architecture = MachOArchitectureConverter.Convert(await streamingBinaryReader.ReadInt()); // cputype
        
        streamingBinaryReader.SkipInt(); // cpusubtype
        var offset = await streamingBinaryReader.ReadPointer(bitness); // offset
        streamingBinaryReader.SkipPointer(bitness); // size
        streamingBinaryReader.SkipPointer(bitness); // align
        if (bitness == Bitness.Bitness64)
            streamingBinaryReader.SkipUInt(); // reserved
        
        return (architecture, offset);
    }
    
    private static async Task<NonFatHeader> ReadNonFatHeaderAsync(StreamingBinaryReader streamingBinaryReader, Bitness bitness)
    {
        var architecture = MachOArchitectureConverter.Convert(await streamingBinaryReader.ReadInt()); // cputype
        streamingBinaryReader.SkipInt(); // cpusubtype
        streamingBinaryReader.SkipUInt(); // filetype
        var numberOfCommands = await streamingBinaryReader.ReadUInt(); // ncmds
        
        streamingBinaryReader.SkipUInt(); // sizeofcmds
        
        streamingBinaryReader.SkipUInt(); // flags
        
        if (bitness == Bitness.Bitness64)
            streamingBinaryReader.SkipUInt(); // reserved

        return new (numberOfCommands, architecture);
    }

    private record struct NonFatHeader(uint NumberOfCommands, Architecture Architecture);
}
