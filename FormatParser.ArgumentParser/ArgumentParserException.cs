namespace FormatParser.ArgumentParser;

public class ArgumentParserException : Exception
{
    public ArgumentParserException()
    {
    }

    public ArgumentParserException(string? message) : base(message)
    {
    }

    public ArgumentParserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}