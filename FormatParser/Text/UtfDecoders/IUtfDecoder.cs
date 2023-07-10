namespace FormatParser.Text;

public interface IUtfDecoder : ITextDecoder
{
    string[] CanReadEncodings { get; }
}