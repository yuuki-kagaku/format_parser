namespace FormatParser;

public class ParsingResult<TData> where TData : class, IData
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
    public static ParsingResult<TData> Success<TData>(TData result) where TData : class, IData
    {
        return new(result, null);
    }
    
    public static ParsingResult<TData> Error<TData>(string error) where TData : class, IData
    {
        return new(null, error);
    }
}