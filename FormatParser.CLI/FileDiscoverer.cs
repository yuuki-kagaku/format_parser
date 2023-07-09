using System.Threading.Channels;
using FormatParser.Helpers;

namespace FormatParser.CLI;

public class FileDiscoverer
{
    public async Task DiscoverFiles(string directory, Channel<string> channel)
    {
        await DiscoverFilesInternal(directory, channel);
        channel.Writer.Complete();
    }
    
    private async Task DiscoverFilesInternal(string directory, Channel<string> channel)
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
        }

        try
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                if (new FileInfo(subDirectory).IsSymlink())
                    continue;

                await DiscoverFilesInternal(subDirectory, channel);
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static bool ShouldSkip(string file)
    {
        var fileInfo = new FileInfo(file);
        return fileInfo.IsSymlink() || fileInfo.IsEmpty();
    }
}