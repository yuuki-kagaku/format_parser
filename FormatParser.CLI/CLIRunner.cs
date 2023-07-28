using System.Diagnostics;
using System.Threading.Channels;
using FormatParser.Helpers;

namespace FormatParser.CLI;

public class CLIRunner
{
    private readonly FileDiscoverer fileDiscoverer;
    private readonly FormatDecoder formatDecoder;
    private readonly IStreamFactory streamFactory;

    public CLIRunner(FileDiscoverer fileDiscoverer, FormatDecoder formatDecoder, IStreamFactory streamFactory)
    {
        this.fileDiscoverer = fileDiscoverer;
        this.formatDecoder = formatDecoder;
        this.streamFactory = streamFactory;
    }

    public async Task Run(FormatParserCliSettings settings, FormatParserCliState state)
    {
        var channel = Channel.CreateUnbounded<string>();

        state.Stopwatch = Stopwatch.StartNew();
        var fileDiscovererTask = RunFileDiscovererWithProcessorContinuation(settings, state, channel);
        var processorTasks = RunProcessors(settings, state, channel);

        await Task.WhenAll(processorTasks.Concat(fileDiscovererTask));
    }

    private IEnumerable<Task> RunProcessors(FormatParserCliSettings settings, FormatParserCliState state, Channel<string> channel)
    {
        for (var i = 0; i < settings.DegreeOfParallelism - 1; i++)
            yield return RunParsingProcessor(channel.Reader, state);
    }
    
    private async Task RunFileDiscovererWithProcessorContinuation(FormatParserCliSettings settings, FormatParserCliState state, Channel<string> channel)
    {
        await Task.Yield();
        await fileDiscoverer.DiscoverFilesAsync(settings.Directory ?? throw new ArgumentNullException(nameof(settings.Directory)), channel);
        await RunParsingProcessor(channel.Reader, state);
    }

    private async Task RunParsingProcessor(ChannelReader<string> channelReader, FormatParserCliState state)
    {
        var processor = new ParsingProcessor(formatDecoder, streamFactory, channelReader, state);
        await Task.Yield();
        await processor.ProcessFiles();
    }
}