namespace FormatParser;

public class ParsingResult<TData> where TData : class
{
    public ParsingResult(TData? result, string? error)
    {
        Result = result;
        ErrorMessage = error;
    }

    public TData? Result { get; }
    public string? ErrorMessage { get; }
}

public static class ParsingResult
{
    public static ParsingResult<TData> Success<TData>(TData result) where TData : class
    {
        return new(result, null);
    }
    
    public static ParsingResult<TData> Error<TData>(string error) where TData : class
    {
        return new(null, error);
    }
}