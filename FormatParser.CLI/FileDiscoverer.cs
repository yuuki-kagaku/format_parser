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
        await FileDiscoveryHelper.RunWithExceptionHandling(async () =>
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (ShouldSkipFile(file))
                    continue;
                
                

                await channelWriter.WriteAsync(file);
            }
        }, settings);
        
        await FileDiscoveryHelper.RunWithExceptionHandling(async () =>
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                if (ShouldSkipDirectory(subDirectory))
                    continue;

                await DiscoverFilesInternalAsync(subDirectory, channelWriter);
            }
        }, settings);
    }
    
    private bool ShouldSkipFile(string file)
    {
        return FileDiscoveryHelper.RunWithExceptionHandling(() =>
        {
            var fileInfo = new FileInfo(file);
            return fileInfo.IsSymlink() || fileInfo.IsEmpty();
        }, true, settings);
    }
    
    private bool ShouldSkipDirectory(string directory) => FileDiscoveryHelper.RunWithExceptionHandling(() =>
        new DirectoryInfo(directory).IsSymlink(), true, settings);
}