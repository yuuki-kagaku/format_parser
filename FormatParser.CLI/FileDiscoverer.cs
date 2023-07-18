using System.Threading.Channels;
using FormatParser.Helpers;

namespace FormatParser.CLI;

public class FileDiscoverer
{
    private readonly FileDiscovererSettings settings;

    public FileDiscoverer(FileDiscovererSettings settings) => this.settings = settings;

    public async Task DiscoverFilesAsync(string directory, Channel<string> channel)
    {
        await DiscoverFilesInternalAsync(directory, channel);
        channel.Writer.Complete();
    }
    
    private async Task DiscoverFilesInternalAsync(string directory, Channel<string> channel)
    {
        try
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (ShouldSkip(file))
                    continue;

                await channel.Writer.WriteAsync(file);
            }
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

        try
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                if (new FileInfo(subDirectory).IsSymlink())
                    continue;

                await DiscoverFilesInternalAsync(subDirectory, channel);
            }
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

    private static bool ShouldSkip(string file)
    {
        var fileInfo = new FileInfo(file);
        return fileInfo.IsSymlink() || fileInfo.IsEmpty();
    }
}