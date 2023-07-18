namespace FormatParser.CLI;

public interface IStreamFactory
{
    Stream GetStream(string filename);
}