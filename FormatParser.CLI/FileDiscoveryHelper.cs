namespace FormatParser.CLI;

public class FileDiscoveryHelper
{
    public static async Task RunWithExceptionHandling(Func<Task> action, FileDiscovererSettings settings)
    {
        try
        {
            await action();
        }
        catch (UnauthorizedAccessException)
        {
            if (settings.FallOnUnauthorizedException)
                throw;
        }
        catch (IOException)
        {
            if (settings.FailOnIOException)
                throw;
        }
    }
    
    public static bool RunWithExceptionHandling(Func<bool> predicate, bool valueOnException, FileDiscovererSettings settings)
    {
        try
        {
            return predicate();
        }
        catch (UnauthorizedAccessException)
        {
            if (settings.FallOnUnauthorizedException)
                throw;
            
            return valueOnException;
        }
        catch (IOException)
        {
            if (settings.FailOnIOException)
                throw;
            
            return valueOnException;
        }
    }
}