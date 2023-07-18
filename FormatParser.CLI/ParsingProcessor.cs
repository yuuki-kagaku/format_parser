using System.Text;
using System.Threading.Channels;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.CLI;

public class ParsingProcessor
{
    private readonly FormatDecoder formatDecoder;
    private readonly ChannelReader<string> channel;
    private readonly FormatParserCliState state;
    private readonly FormatParserCliSettings settings;
    private readonly CancellationToken cancellationToken;

    public ParsingProcessor(FormatDecoder formatDecoder, ChannelReader<string> channel, FormatParserCliState state, FormatParserCliSettings settings, CancellationToken cancellationToken)
    {
        this.formatDecoder = formatDecoder;
        this.channel = channel;
        this.state = state;
        this.settings = settings;
        this.cancellationToken = cancellationToken;
    }

    public async Task ProcessFiles()
    {
        while (true)
        {
            while (channel.TryRead(out var file))
            {
                try
                {
                    var fileInfo = await formatDecoder.DecodeFile(file);
                    AddInfoToState(fileInfo);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            
            var hasMoreDate = await channel.WaitToReadAsync(cancellationToken);
            
            if (!hasMoreDate)
                return;
        }
    }

    private void AddInfoToState(IFileFormatInfo info)
    {
        state.Occurence.GetOrAdd(info, _ => new()).Increment();
    }
}