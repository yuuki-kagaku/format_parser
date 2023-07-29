namespace FormatParser.CLI;

public class AtomicInt
{
    private volatile int value;

    public int Add(int val) => Interlocked.Add(ref this.value, val);
    public int Increment() => Interlocked.Increment(ref value);

    public int Read() => Volatile.Read(ref value);
    
    public static implicit operator int (AtomicInt atomicInt) => atomicInt.Read();
}
