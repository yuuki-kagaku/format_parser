namespace FormatParser.Text;

public interface ITextBasedFormatDetector
{
    public bool TryMatchFormat(string text, out string encoding);
    
    public string MimeType { get; }
}