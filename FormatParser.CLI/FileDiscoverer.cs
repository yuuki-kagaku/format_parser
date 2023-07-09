using System.Threading.Channels;

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
                if (IsSymlink(new FileInfo(subDirectory)))
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
        return IsSymlink(fileInfo) || IsEmpty(fileInfo);
    }
    
    private static bool IsSymlink(FileInfo fileInfo) => fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
    private static bool IsEmpty(FileInfo fileInfo) => fileInfo.Length == 0;
}