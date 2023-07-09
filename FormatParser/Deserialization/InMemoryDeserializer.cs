namespace FormatParser;

public class InMemoryDeserializer
{
    private int offset = 0;
    private readonly byte[] buffer;
    private readonly int length;
    
    private Endianness endianness = Endianness.LittleEndian;

    public InMemoryDeserializer(byte[] buffer)
    {
        this.buffer = buffer;
        length = buffer.Length;
    }
    
    public InMemoryDeserializer(ArraySegment<byte> arraySegment)
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

    public void SetEndianess(Endianness endianness)
    {
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
            throw new DeserializerException("Does not have enough capacity to read.");

        ushort result;
        fixed (void* ptr = &buffer[offset])
            result = *(ushort*) ptr;
        
        offset += sizeof (ushort);
        return result;
    }
    
    public unsafe byte ReadByte()
    {
        if (!CanRead(sizeof(byte)))
            throw new DeserializerException("Does not have enough capacity to read.");

        byte result;
        fixed (void* ptr = &buffer[offset])
            result = *(byte*) ptr;
        
        offset += sizeof (byte);
        return result;
    }
    
    public bool CanRead(int size) => offset + size <= length;
}