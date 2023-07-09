using System.Threading.Channels;

namespace FormatParser.CLI;

public class ParsingProcessor
{
    public CancellationToken CancellationToken { get; }
    private readonly FormatDecoder formatDecoder;
    private readonly ChannelReader<string> channel;
    private readonly ForamatParserCliState state;

    public ParsingProcessor(FormatDecoder formatDecoder, ChannelReader<string> channel, ForamatParserCliState state, CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        this.formatDecoder = formatDecoder;
        this.channel = channel;
        this.state = state;
    }

    public async Task ProcessFiles()
    {
        while (true)
        {
            while (channel.TryRead(out var file))
            {
                var fileInfo = await formatDecoder.DecodeFile(file);
                AddInfoToState(fileInfo);
            }
            
            var hasMoreDate = await channel.WaitToReadAsync(CancellationToken);
            
            if (!hasMoreDate)
                return;
        }
    }

    private void AddInfoToState(IFileFormatInfo info)
    {
        state.Occurence.GetOrAdd(info, _ => new()).Increment();
    }
}