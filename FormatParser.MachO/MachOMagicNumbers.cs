using System.Collections.Immutable;
using FormatParser.Domain;
using FormatParser.Helpers;

namespace FormatParser.MachO;

internal static class MachOMagicNumbers
{
    public static readonly IReadOnlyDictionary<byte[], (Bitness bitness, Endianness Endianess, bool IsFat)> NonFat =
        new Dictionary<byte[], (Bitness bitness, Endianness Endianess, bool IsFat)>(ArrayComparer<byte>.Instance)
        {
            { new byte[] { 0xFE, 0xED, 0xFA, 0xCE }, (Bitness.Bitness32, Endianness.BigEndian, false) },
            { new byte[] { 0xCE, 0xFA, 0xED, 0xFE }, (Bitness.Bitness32, Endianness.LittleEndian, false) },
            { new byte[] { 0xFE, 0xED, 0xFA, 0xCF }, (Bitness.Bitness64, Endianness.BigEndian, false) },
            { new byte[] { 0xCF, 0xFA, 0xED, 0xFE }, (Bitness.Bitness64, Endianness.LittleEndian, false) },
        }.ToImmutableDictionary(ArrayComparer<byte>.Instance);

    public static readonly IReadOnlyDictionary<byte[], (Bitness bitness, Endianness Endianess, bool IsFat)>
        All = NonFat
            .ToImmutableDictionary(ArrayComparer<byte>.Instance)
            .Add(new byte[] { 0xCA, 0xFE, 0xBA, 0xBE }, (Bitness.Bitness32, Endianness.BigEndian, true))
            .Add(new byte[] { 0xBE, 0xBA, 0xFE, 0xCA }, (Bitness.Bitness32, Endianness.LittleEndian, true))
            .Add(new byte[] { 0xCA, 0xFE, 0xBA, 0xBF }, (Bitness.Bitness64, Endianness.BigEndian, true))
            .Add(new byte[] { 0xBF, 0xBA, 0xFE, 0xCA }, (Bitness.Bitness64, Endianness.LittleEndian, true));
}