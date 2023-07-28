using System.Threading.Channels;
using FormatParser.Helpers;

namespace FormatParser.CLI;

public class FileDiscoverer
{
    private readonly FileDiscovererSettings settings;

    public FileDiscoverer(FileDiscovererSettings settings) => this.settings = settings;

    public async Task DiscoverFilesAsync(string directory, ChannelWriter<string> channelWriter)
    {
        await DiscoverFilesInternalAsync(directory, channelWriter);
        channelWriter.Complete();
    }
    
    private async Task DiscoverFilesInternalAsync(string directory, ChannelWriter<string> channelWriter)
    {
        await RunWithExceptionHandling(async () =>
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (ShouldSkipFile(file))
                    continue;

                await channelWriter.WriteAsync(file);
            }
        });
        
        await RunWithExceptionHandling(async () =>
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                if (ShouldSkipDirectory(subDirectory))
                    continue;

                await DiscoverFilesInternalAsync(subDirectory, channelWriter);
            }
        });
    }
    
    private bool ShouldSkipFile(string file)
    {
        return RunWithExceptionHandling(() =>
        {
            var fileInfo = new FileInfo(file);
            return fileInfo.IsSymlink() || fileInfo.IsEmpty();
        }, true);
    }
    
    private bool ShouldSkipDirectory(string directory) => RunWithExceptionHandling(() => new DirectoryInfo(directory).IsSymlink(), true);

    private async Task RunWithExceptionHandling(Func<Task> action)
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
    
    private bool RunWithExceptionHandling(Func<bool> action, bool valueOnException)
    {
        try
        {
            return action();
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