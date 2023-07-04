namespace FormatParser.MachO;

public record MachOData(Endianess Endianess, Bitness Bitness, Architecture Architecture, bool Signed) : IData;