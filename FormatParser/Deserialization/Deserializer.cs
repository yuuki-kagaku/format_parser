using System.Text;

namespace FormatParser;

public class Deserializer
{
    private readonly Stream stream;
    private readonly byte[] defaultBuffer = GC.AllocateArray<byte>(16, true);
    private Endianess endianess = Endianess.LittleEndian;
    private static readonly Endianess runningCpuEndianess = BitConverter.IsLittleEndian ? Endianess.LittleEndian : Endianess.BigEndian;

    public Deserializer(Stream stream)
    {
        this.stream = stream;
    }

    public void SetEndianess(Endianess endianess)
    {
        this.endianess = endianess;
    }

    public long Offset
    {
        get => stream.Position;
        set => stream.Position = value;
    }
    
    public async Task<byte[]> ReadBytes(int count)
    {
        var array = new byte[count];
        await ReadInternalAsync(count, array, true);
        return array;
    }
    
    public async Task<ArraySegment<byte>> TryReadBytes(int count)
    {
        var array = new byte[count];
        var readBytes = await ReadInternalAsync(count, array, false);
        return new ArraySegment<byte>(array, 0, readBytes);
    }

    public async Task<byte> ReadByte()
    {
        await ReadInternalAsync(sizeof(byte));
        return defaultBuffer[0];
    }

    public void SkipUlong() => SkipLong();

    public void SkipLong() => Offset += sizeof(long);

    public void SkipUInt() => SkipInt();
    public void SkipInt() => Offset += sizeof(int);

    public void SkipUShort() => SkipShort();
    public void SkipShort() => Offset += sizeof(short);
    public void SkipByte() => Offset += 1;
    public void SkipBytes(int count) => Offset += count;
    public void SkipBytes(uint count) => Offset += count;

    public async Task<string> ReadNulTerminatingStringAsync(int size)
    {
        var array = new byte[size];
        await ReadInternalAsync(size, array, true);

        if (array[^1] != 0)
            throw new Exception();

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
    
    private unsafe short ConvertShort()
    {
        if (endianess != runningCpuEndianess)
            InvertByteForShort();
        
        fixed (byte* ptr = defaultBuffer)
            return *(short*)ptr;
    }
    
    private unsafe ushort ConvertUShort()
    {
        if (endianess != runningCpuEndianess)
            InvertByteForShort();
        
        fixed (byte* ptr = defaultBuffer)
            return *(ushort*)ptr;
    }
    
    private unsafe int ConvertInt()
    {
        if (endianess != runningCpuEndianess)
            InvertByteForInt();
        
        fixed (byte* ptr = defaultBuffer)
            return *(int*)ptr;
    }
    
    private unsafe uint ConvertUInt()
    {
        if (endianess != runningCpuEndianess)
            InvertByteForInt();
        
        fixed (byte* ptr = defaultBuffer)
            return *(uint*)ptr;
    }
    
    private unsafe ulong ConvertULong()
    {
        if (endianess != runningCpuEndianess)
            InvertBytesForLong();
        
        fixed (byte* ptr = defaultBuffer)
            return *(ulong*)ptr;
    }
    
    private unsafe long ConvertLong()
    {
        if (endianess != runningCpuEndianess)
            InvertBytesForLong();
        
        fixed (byte* ptr = defaultBuffer)
            return *(long*)ptr;
    }
    
    private void InvertByteForShort()
    {
        (defaultBuffer[0], defaultBuffer[1]) = (defaultBuffer[1], defaultBuffer[0]);
    }
    
    private void InvertByteForInt()
    {
        (defaultBuffer[0], defaultBuffer[3]) = (defaultBuffer[3], defaultBuffer[0]);
        (defaultBuffer[1], defaultBuffer[2]) = (defaultBuffer[2], defaultBuffer[1]);
    }

    private void InvertBytesForLong()
    {
        (defaultBuffer[0], defaultBuffer[7]) = (defaultBuffer[7], defaultBuffer[0]);
        (defaultBuffer[1], defaultBuffer[6]) = (defaultBuffer[6], defaultBuffer[1]);
        (defaultBuffer[2], defaultBuffer[5]) = (defaultBuffer[5], defaultBuffer[2]);
        (defaultBuffer[3], defaultBuffer[4]) = (defaultBuffer[4], defaultBuffer[3]);
    }
    
    public async Task<ulong> ReadPointer(Bitness bitness)
    {
        if (bitness == Bitness.Bitness32)
            return await ReadUInt();

        return await ReadULong();
    }
    
    public void SkipPointer(Bitness bitness)
    {
        if (bitness == Bitness.Bitness32)
        {
            SkipInt();
            return;
        }

        SkipLong();
    }

    private Task ReadInternalAsync(int count) => ReadInternalAsync(count, defaultBuffer, true);

    private async Task<int> ReadInternalAsync(int count, byte[] buffer, bool ensureSize)
    {
        var totalBytesRead = 0;

        while (totalBytesRead != count)
        {
            var bytesRead = await ReadInternalAsync(buffer, totalBytesRead, count - totalBytesRead);

            if (bytesRead == 0)
                if (ensureSize)
                    throw new DeserializerException("Not enough data in stream.");
                else
                    return bytesRead;

            totalBytesRead += bytesRead;
        }

        return count;
    }
    
    private Task<int> ReadInternalAsync(byte[] dest, int offset, int count) => stream.ReadAsync(dest, offset, count);
}