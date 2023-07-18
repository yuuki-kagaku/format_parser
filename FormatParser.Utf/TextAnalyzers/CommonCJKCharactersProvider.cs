using System.Text.Json;

namespace FormatParser.Text.TextAnalyzers;

public class CommonCJKCharactersProvider
{
    public CommonCJKCharactersProvider()
    {
        var settings = ReadSettings();
        MostUsedHangul = File.ReadAllText(settings.MostUsedHangul);
        MostUsedKanji = File.ReadAllText(settings.MostUsedKanji);
        MostUsedChineseCharacters = File.ReadAllText(settings.MostUsedChineseCharacters);
    }

    public IEnumerable<char> MostUsedHangul { get; }
    public IEnumerable<char> MostUsedKanji { get; }
    public IEnumerable<char> MostUsedChineseCharacters { get; }

    private CommomCJKCharatersSettings ReadSettings()
    {
        if (!File.Exists(SettingsFile))
            return new();

        var json = File.ReadAllText(SettingsFile);

        return JsonSerializer.Deserialize<CommomCJKCharatersSettings>(json) ?? throw new FormatParserException("Failed to read commoN CJK characters settings");
    }

    private const string SettingsFile = "CommomCJKCharactersSettings.json";
}