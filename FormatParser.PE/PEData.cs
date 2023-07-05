namespace FormatParser.PE;

public record PEData(Bitness Bitness, Architecture Architecture, bool IsManaged) : IData;