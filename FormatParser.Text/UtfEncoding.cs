namespace FormatParser.Text;

public enum UtfEncoding
{
    Unknown,
    
    Utf8BOM,
    Utf8NoBOM,
    
    UTF16LeNoBom,
    UTF16LeBom,
    UTF16BeNoBom,
    UTF16BeBom,
    
    UTF32LeNoBom,
    UTF32LeBom,
    UTF32BeNoBom,
    UTF32BeBom,
}