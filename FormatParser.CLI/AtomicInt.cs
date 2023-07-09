namespace FormatParser.CLI;

public class AtomicInt
{
    private volatile int value;

    public int Add(int value) => Interlocked.Add(ref this.value, value);
    public int Increment() => Interlocked.Increment(ref this.value);

    public int Read() => value;
    
    public static implicit operator int (AtomicInt atomicInt) => atomicInt.Read();
}