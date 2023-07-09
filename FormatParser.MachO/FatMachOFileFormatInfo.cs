using System.Collections.Immutable;
using FormatParser.Helpers;

namespace FormatParser.MachO;

public record FatMachOFileFormatInfo (Endianness Endianness, Bitness Bitness, ImmutableArray<MachOFileFormatInfo> Datas) : IFileFormatInfo
{
    public virtual bool Equals(IFileFormatInfo? other) => other is FatMachOFileFormatInfo textFileFormatInfo && this.Equals(textFileFormatInfo);

    public virtual bool Equals(FatMachOFileFormatInfo? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return Endianness == other.Endianness && Bitness == other.Bitness && Datas.SequenceEqual(other.Datas);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Endianness, (int)Bitness, SequentialCollectionComparer<MachOFileFormatInfo>.Instance.GetHashCode(Datas));
    }

    public string ToPrettyString()
    {
        return $"Fat Mach-O {Endianness} {Bitness}: {string.Join(',', Datas.Select(x => x))}";
    }
}