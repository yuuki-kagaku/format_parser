namespace FormatParser.Domain;

public enum Architecture
{
    Unknown,
    
    I386,
    Amd64,
    
    Arm, // AARCH32
    Arm64,
    
    Ia64,
    M68K,

    PowerPC,
    Ppc64,
    
    Sparc,
        
    Mips,
    S390, // IBM System Z
    
    RiscV,
}