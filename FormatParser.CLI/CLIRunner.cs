using System.Diagnostics;
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

    public async Task Run(FormatParserCliSettings settings, FormatParserCliState state, CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<string>();

        state.Stopwatch = Stopwatch.StartNew();
        var discovererTask = RunFileDiscovererWithProcessorContinuation(settings, state, channel, cancellationToken);
        var processorTasks = RunProcessors(settings, state, channel, cancellationToken);

        await Task.WhenAll(processorTasks.Concat(new[] { discovererTask }));
    }

    private List<Task> RunProcessors(FormatParserCliSettings settings, FormatParserCliState state, Channel<string> channel, CancellationToken cancellationToken)
    {
        var list = new List<Task>();
        
        for (var i = 0; i < settings.DegreeOfParallelism - 1; i++)
            list.Add(RunParsingProcessor(channel.Reader, state, settings, cancellationToken));
        
        return list;
    }
    
    private async Task RunFileDiscovererWithProcessorContinuation(FormatParserCliSettings settings, FormatParserCliState state, Channel<string> channel, CancellationToken cancellationToken)
    {
        await Task.Yield();
        await fileDiscoverer.DiscoverFilesAsync(settings.Directory, channel);
        await RunParsingProcessor(channel.Reader, state, settings, cancellationToken);
    }

    private async Task RunParsingProcessor(ChannelReader<string> channelReader, FormatParserCliState state, FormatParserCliSettings settings, CancellationToken cancellationToken)
    {
        var processor = new ParsingProcessor(formatDecoder, channelReader, state, settings, cancellationToken);
        await Task.Yield();
        await processor.ProcessFiles();
    }
}