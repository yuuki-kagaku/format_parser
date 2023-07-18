using System.Text.Json;

namespace FormatParser.Text.EncodingAnalyzers;

public class CommonCJKCharactersProvider
{
    private readonly Lazy<IEnumerable<char>> mostUsedHangul;
    private readonly Lazy<IEnumerable<char>> mostUsedKanji;
    private readonly Lazy<IEnumerable<char>> mostUsedChineseCharacters;

    public CommonCJKCharactersProvider()
    {
        var settings = new Lazy<CommomCJKCharatersSettings>(() => ReadSettings(), LazyThreadSafetyMode.PublicationOnly);
        mostUsedHangul = new Lazy<IEnumerable<char>>(() => File.ReadAllText(settings.Value.MostUsedHangul));
        mostUsedKanji = new Lazy<IEnumerable<char>>(() => File.ReadAllText(settings.Value.MostUsedKanji));
        mostUsedChineseCharacters = new Lazy<IEnumerable<char>>(() => File.ReadAllText(settings.Value.MostUsedChineseCharacters));
    }

    public IEnumerable<char> MostUsedHangul => mostUsedHangul.Value;
    public IEnumerable<char> MostUsedKanji => mostUsedKanji.Value;
    public IEnumerable<char> MostUsedChineseCharacters => mostUsedChineseCharacters.Value;

    private CommomCJKCharatersSettings ReadSettings()
    {
        if (!File.Exists(SettingsFile))
            return new();

        var json = File.ReadAllText(SettingsFile);

        return JsonSerializer.Deserialize<CommomCJKCharatersSettings>(json) ?? throw new FormatParserException("Failed to read commoN CJK characters settings");
    }

    private const string SettingsFile = "CommomCJKCharactersSettings.json";
}