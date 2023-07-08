namespace FormatParser;

public class InMemoryDeserializer
{
    private int offset = 0;
    private byte[] buffer;
    private Endianess endianess = Endianess.LittleEndian;

    public InMemoryDeserializer(byte[] buffer)
    {
        this.buffer = buffer;
    }

    public void SetEndianess(Endianess endianess)
    {
        this.endianess = endianess;
    }
    
    public int Offset
    {
        get => offset;
        set => offset = value;
    }

    public byte[] ReadBytes(int count)
    {
        if (offset + count <= buffer.Length)
            count = buffer.Length - offset;

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
    
    public bool CanRead(int size) => offset + size <= buffer.Length;
}