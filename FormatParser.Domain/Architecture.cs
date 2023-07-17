namespace FormatParser;

public enum Architecture
{
    Unknown,
    
    i386,
    Amd64,
    
    Arm, // AARCH32
    Arm64,
    
    ia64,
    m68k,

    PowerPC,
    ppc64,
    
    Sparc,
        
    mips,
    s390, // IBM System Z
    
    RiscV,
}