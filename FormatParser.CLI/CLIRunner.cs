using System.Threading.Channels;

namespace FormatParser.CLI;

public class CLIRunner
{
    private readonly FileDiscoverer fileDiscoverer;
    private readonly FormatDecoder formatDecoder;

    public CLIRunner(FileDiscoverer fileDiscoverer, FormatDecoder formatDecoder)
    {
        this.fileDiscoverer = fileDiscoverer;
        this.formatDecoder = formatDecoder;
    }

    public async Task Run(FormatParserCliSettings settings, ForamatParserCliState state, CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<string>();

        await fileDiscoverer.DiscoverFiles(settings.Directory, channel);
       
        await RunParsingProcessor(channel.Reader, state, cancellationToken);
    }

    private Task RunParsingProcessor(ChannelReader<string> channelReader, ForamatParserCliState state, CancellationToken cancellationToken)
    {
        var processor = new ParsingProcessor(formatDecoder, channelReader, state, cancellationToken);
        return processor.ProcessFiles();
    }
}