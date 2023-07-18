using System.Buffers.Binary;
using System.Text;
using FormatParser.Domain;

namespace FormatParser.BinaryReader;

public class StreamingBinaryReader
{
    private readonly Stream stream;
    private readonly byte[] defaultBuffer = GC.AllocateArray<byte>(16, true);
    private Endianness endianness;
    private static readonly Endianness RunningCpuEndianness = BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;

    public StreamingBinaryReader(Stream stream, Endianness endianness)
    {
        this.stream = stream;
        this.endianness = endianness;
    }

    public void SetEndianness(Endianness e)
    {
        if (e is not (Endianness.BigEndian or Endianness.LittleEndian))
            throw new ArgumentOutOfRangeException(nameof(e));
        
        endianness = e;
    }

    public long Offset
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public long Length => stream.Length;
    
    public async Task<byte[]> ReadBytesAsync(int count)
    {
        var array = new byte[count];
        await ReadInternalAsync(count, array, true);
        return array;
    }
    
    public async Task<ArraySegment<byte>> TryReadArraySegment(int count)
    {
        var array = new byte[count];
        var readBytes = await ReadInternalAsync(count, array, false);
        
        return new ArraySegment<byte>(array, 0, readBytes);
    }

    public void SkipUlong() => SkipLong();

    public void SkipLong() => Skip<long>();

    public void SkipUInt() => SkipInt();
    public void SkipInt() => Skip<int>();

    public void SkipUShort() => SkipShort();
    public void SkipShort() => Skip<short>();
    public void SkipByte() => Skip<byte>();
    public void SkipBytes(int count) => Offset += count;
    public void SkipBytes(uint count) => Offset += count;
    private unsafe void Skip<T>() where T : unmanaged => Offset += sizeof(T);

    public async Task<string> ReadNulTerminatingStringAsync(int size)
    {
        var array = new byte[size];
        await ReadInternalAsync(size, array, true);

        if (array[^1] != 0)
            throw new BinaryReaderException("Expected to read null terminating string, but string does not terminates with \\0 at expected length.");

        return Encoding.ASCII.GetString(new ArraySegment<byte>(array, 0, array.Length - 1));
    }
    
    public async Task<short> ReadShort()
    {
        await ReadInternalAsync(sizeof(short));
        return ConvertShort();
    }
    
    public async Task<ushort> ReadUShort()
    {
        await ReadInternalAsync(sizeof(ushort));
        return ConvertUShort();
    }
    
    public async Task<int> ReadInt()
    {
        await ReadInternalAsync(sizeof(int));
        return ConvertInt();
    }
    
    public async Task<uint> ReadUInt()
    {
        await ReadInternalAsync(sizeof(uint));
        return ConvertUInt();
    }
    
    public async Task<long> ReadLong()
    {
        await ReadInternalAsync(sizeof(long));
        return ConvertLong();
    }
    
    public async Task<ulong> ReadULong()
    {
        await ReadInternalAsync(sizeof(ulong));
        return ConvertULong();
    }
    
    public async Task<ulong> ReadPointer(Bitness bitness)
    {
        return bitness switch
        {
            Bitness.Bitness16 => await ReadUShort(),
            Bitness.Bitness32 => await ReadUInt(),
            Bitness.Bitness64 => await ReadULong(),
            _ => throw new ArgumentOutOfRangeException(nameof(bitness))
        };
    }
    
    public void SkipPointer(Bitness bitness)
    {
        switch (bitness)
        {
            case Bitness.Bitness32:
                SkipInt();
                return;
            case Bitness.Bitness64:
                SkipLong();
                return;
            case Bitness.Bitness16:
                SkipShort();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(bitness));
        }
    }
    
    private unsafe short ConvertShort()
    {
        fixed (void* ptr = defaultBuffer)
            return ReverseEndiannessIfNeeded(*(short*)ptr);
    }
    
    private unsafe ushort ConvertUShort()
    {
        fixed (void* ptr = defaultBuffer)
            return ReverseEndiannessIfNeeded(*(ushort*)ptr);
    }
    
    private unsafe int ConvertInt()
    {
        fixed (void* ptr = defaultBuffer)
            return ReverseEndiannessIfNeeded(*(int*)ptr);
    }
    
    private unsafe uint ConvertUInt()
    {
        fixed (void* ptr = defaultBuffer)
            return ReverseEndiannessIfNeeded(*(uint*)ptr);
    }
    
    private unsafe ulong ConvertULong()
    {
        fixed (void* ptr = defaultBuffer)
            return ReverseEndiannessIfNeeded(*(ulong*)ptr);
    }
    
    private unsafe long ConvertLong()
    {
        fixed (void* ptr = defaultBuffer)
            return ReverseEndiannessIfNeeded(*(long*)ptr);
    }

    private ushort ReverseEndiannessIfNeeded(ushort val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);
    
    private uint ReverseEndiannessIfNeeded(uint val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);
    
    private ulong ReverseEndiannessIfNeeded(ulong val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);
    
    private short ReverseEndiannessIfNeeded(short val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);
    
    private int ReverseEndiannessIfNeeded(int val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);
    
    private long ReverseEndiannessIfNeeded(long val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);

    private Task ReadInternalAsync(int count) => ReadInternalAsync(count, defaultBuffer, true);

    private async Task<int> ReadInternalAsync(int count, byte[] buffer, bool ensureAllBytesRead)
    {
        var totalBytesRead = 0;

        while (totalBytesRead != count)
        {
            var bytesRead = await stream.ReadAsync(buffer, totalBytesRead, count - totalBytesRead);

            if (bytesRead == 0)
                return ensureAllBytesRead ? throw new BinaryReaderException("Not enough data in stream.") : totalBytesRead;

            totalBytesRead += bytesRead;
        }

        return count;
    }
}