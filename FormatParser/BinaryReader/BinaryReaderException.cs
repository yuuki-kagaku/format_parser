namespace FormatParser;

public class BinaryReaderException : Exception
{
    public BinaryReaderException()
    {
    }

    public BinaryReaderException(string? message) : base(message)
    {
    }

    public BinaryReaderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}