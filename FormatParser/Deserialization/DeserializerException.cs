namespace FormatParser;

public class DeserializerException : Exception
{
    public DeserializerException()
    {
    }

    public DeserializerException(string? message) : base(message)
    {
    }

    public DeserializerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}