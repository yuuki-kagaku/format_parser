namespace FormatParser.ELF;

public record ElfData(Endianess Endianess, Bitness Bitness, Architecture Architecture, string? Interpreter);