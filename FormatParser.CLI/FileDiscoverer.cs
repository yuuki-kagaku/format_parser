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
        try
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (ShouldSkip(file))
                    continue;

                await channelWriter.WriteAsync(file);
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

                await DiscoverFilesInternalAsync(subDirectory, channelWriter);
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