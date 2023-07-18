namespace FormatParser.CLI;

public class StreamFactory : IStreamFactory
{
    private readonly int fileStreamBufferSize;

    public StreamFactory(int fileStreamBufferSize) => this.fileStreamBufferSize = fileStreamBufferSize;

    public Stream GetStream(string filename) => new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, fileStreamBufferSize);
}