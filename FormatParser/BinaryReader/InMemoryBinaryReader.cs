using System.Buffers.Binary;

namespace FormatParser;

public class InMemoryBinaryReader
{
    private int offset = 0;
    private readonly byte[] buffer;
    private readonly int length;
    private static readonly Endianness RunningCpuEndianness = BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;

    private Endianness endianness = Endianness.LittleEndian;

    public InMemoryBinaryReader(byte[] buffer)
    {
        this.buffer = buffer;
        length = buffer.Length;
    }
    
    public int Length => length;

    public InMemoryBinaryReader(ArraySegment<byte> arraySegment)
    {
        if (arraySegment.Offset == 0)
        {
            this.buffer = arraySegment.Array!;
            this.length = arraySegment.Count;
        }
        else
        {
            this.buffer = arraySegment.ToArray();
            this.length = this.buffer.Length;
        }
    }

    public void SetEndianness(Endianness endianness)
    {
        if (endianness is not (Endianness.BigEndian or Endianness.LittleEndian))
            throw new ArgumentOutOfRangeException(nameof(endianness));
        
        this.endianness = endianness;
    }
    
    public int Offset
    {
        get => offset;
        set => offset = value;
    }

    public byte[] ReadBytes(int count)
    {
        if (offset + count <= length)
            count = length - offset;

        var result = buffer[offset..count];
        offset += count;
        return result;
    }
    
    public unsafe bool TryReadUShort(out ushort result)
    {
        result = 0;
        if (!CanRead(sizeof(ushort)))
            return false;
        
        fixed (void* ptr = &buffer[offset])
            result = *(ushort*) ptr;
        
        offset += sizeof (ushort);
        
        result = ReverseEndiannessIfNeeded(result);
        return true;
    }
    
    public unsafe bool TryReadUInt(out uint result)
    {
        result = 0;
        if (!CanRead(sizeof(ushort)))
            return false;
        
        fixed (void* ptr = &buffer[offset])
            result = *(uint*) ptr;
        
        offset += sizeof(uint);
        
        result = ReverseEndiannessIfNeeded(result);
        return true;
    }
    
    public unsafe bool TryReadByte(out byte result)
    {
        result = 0;
        if (!CanRead(sizeof(byte)))
            return false;
        
        fixed (void* ptr = &buffer[offset])
            result = *(byte*) ptr;
        
        offset += sizeof (byte);
        return true;
    }
    
    public unsafe ushort ReadUShort()
    {
        if (!CanRead(sizeof(ushort)))
            throw new BinaryReaderException("Does not have enough capacity to read.");

        ushort result;
        fixed (void* ptr = &buffer[offset])
            result = *(ushort*) ptr;
        
        offset += sizeof (ushort);

        result = ReverseEndiannessIfNeeded(result);
        return result;
    }
    
    public unsafe uint ReadUInt()
    {
        if (!CanRead(sizeof(uint)))
            throw new BinaryReaderException("Does not have enough capacity to read.");

        uint result;
        fixed (void* ptr = &buffer[offset])
            result = *(uint*) ptr;
        
        offset += sizeof (uint);

        result = ReverseEndiannessIfNeeded(result);
        return result;
    }
    
    public unsafe byte ReadByte()
    {
        if (!CanRead(sizeof(byte)))
            throw new BinaryReaderException("Does not have enough capacity to read.");

        byte result;
        fixed (void* ptr = &buffer[offset])
            result = *(byte*) ptr;
        
        offset += sizeof (byte);
        return result;
    }

    private ushort ReverseEndiannessIfNeeded(ushort val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);
    
    private uint ReverseEndiannessIfNeeded(uint val) => 
        endianness == RunningCpuEndianness ? val : BinaryPrimitives.ReverseEndianness(val);

    public bool CanRead(int size) => offset + size <= length;
}