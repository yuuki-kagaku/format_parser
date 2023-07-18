using System.Text;
using System.Threading.Channels;
using FormatParser.Domain;
using FormatParser.Text;

namespace FormatParser.CLI;

public class ParsingProcessor
{
    private readonly FormatDecoder formatDecoder;
    private readonly IStreamFactory streamFactory;
    private readonly ChannelReader<string> channel;
    private readonly FormatParserCliState state;

    public ParsingProcessor(FormatDecoder formatDecoder, IStreamFactory streamFactory, ChannelReader<string> channel, FormatParserCliState state)
    {
        this.formatDecoder = formatDecoder;
        this.streamFactory = streamFactory;
        this.channel = channel;
        this.state = state;
    }

    public async Task ProcessFiles()
    {
        while (true)
        {
            while (channel.TryRead(out var file))
            {
                try
                {
                    await using var stream = streamFactory.GetStream(file);
                    var fileInfo = await formatDecoder.Decode(stream);
                    AddInfoToState(fileInfo);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            
            var hasMoreDate = await channel.WaitToReadAsync();
            
            if (!hasMoreDate)
                return;
        }
    }

    private void AddInfoToState(IFileFormatInfo info)
    {
        state.Occurence.GetOrAdd(info, _ => new()).Increment();
    }
}