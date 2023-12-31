using System.Threading.Channels;
using FormatParser.Domain;
using FormatParser.Helpers;

namespace FormatParser.CLI;

public class ParsingProcessor
{
    private readonly FormatDetector formatDetector;
    private readonly IStreamFactory streamFactory;
    private readonly ChannelReader<string> channelReader;
    private readonly FormatParserCliState state;
    private readonly FileDiscovererSettings settings;

    public ParsingProcessor(FormatDetector formatDetector, IStreamFactory streamFactory, ChannelReader<string> channelReader, FormatParserCliState state, FileDiscovererSettings settings)
    {
        this.formatDetector = formatDetector;
        this.streamFactory = streamFactory;
        this.channelReader = channelReader;
        this.state = state;
        this.settings = settings;
    }

    public async Task ProcessFiles()
    {
        while (true)
        {
            while (channelReader.TryRead(out var file))
            {
                await FileDiscoveryHelper.RunWithExceptionHandling(async () =>
                {
                    await using var stream = streamFactory.GetStream(file);
                    var fileFormatInfo = await formatDetector.DetectAsync(stream);
                    AddInfoToState(fileFormatInfo);
                }, settings);
            }
            
            var hasMoreData = await channelReader.WaitToReadAsync();
            
            if (!hasMoreData)
                return;
        }
    }

    private void AddInfoToState(IFileFormatInfo info)
    {
        state.Occurence.GetOrNew(info).Increment();
    }
}