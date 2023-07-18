namespace FormatParser.CLI;

public class AtomicInt
{
    private volatile int value;

    public int Add(int value) => Interlocked.Add(ref this.value, value);
    public int Increment() => Interlocked.Increment(ref value);

    public int Read() => Volatile.Read(ref value);
    
    public static implicit operator int (AtomicInt atomicInt) => atomicInt.Read();
}