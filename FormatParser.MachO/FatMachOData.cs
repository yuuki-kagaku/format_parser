using System.Collections.Immutable;

namespace FormatParser.MachO;

public record FatMachOData(Endianess Endianess, Bitness Bitness, ImmutableArray<MachOData> Datas) : IData;