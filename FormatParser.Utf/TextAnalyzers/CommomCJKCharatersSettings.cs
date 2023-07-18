namespace FormatParser.Text.TextAnalyzers;

public class CommomCJKCharatersSettings
{
    public string MostUsedHangul { get; set; } = $"dictionaries{Path.DirectorySeparatorChar}most_used_hangul";
    public string MostUsedKanji { get; set; } = $"dictionaries{Path.DirectorySeparatorChar}most_used_kanji";
    
    public string MostUsedChineseCharacters { get; set; } = $"dictionaries{Path.DirectorySeparatorChar}most_used_chinese_characters";
}