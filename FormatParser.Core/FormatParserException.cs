namespace FormatParser;

public class FormatParserException : Exception
{
    public FormatParserException()
    {
    }

    public FormatParserException(string? message) : base(message)
    {
    }

    public FormatParserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}