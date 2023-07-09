using System.Collections.Immutable;
using FormatParser.Helpers;

namespace FormatParser.MachO;

public record FatMachOFileFormatInfo (Endianess Endianess, Bitness Bitness, ImmutableArray<MachOFileFormatInfo> Datas) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other)
    {
        if (other is not FatMachOFileFormatInfo textFileFormatInfo)
            return false;

        return this.Equals(textFileFormatInfo);
    }

    public virtual bool Equals(FatMachOFileFormatInfo? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return Endianess == other.Endianess && Bitness == other.Bitness && Datas.SequenceEqual(other.Datas);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Endianess, (int)Bitness, SequentialCollectionComparer<MachOFileFormatInfo>.Instance.GetHashCode(Datas));
    }

    public string ToPrettyString()
    {
        return $"Fat Mach-O {Endianess} {Bitness}: {string.Join(',', Datas.Select(x => x))}";
    }
}