using System.Diagnostics.CodeAnalysis;

namespace FormatParser.Domain;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Architecture
{
    Unknown,
    
    I386,
    Amd64,
    
    Arm,
    Arm64,
    
    Ia64,
    M68K,

    PowerPcBigEndian,
    PowerPcLittleEndian,
    Ppc64,
    
    Sparc,
        
    MipsLittleEndian,
    MipsBigEndian,
    S390,
    S390x,
    
    RiscV,
}