namespace FormatParser.Windows1251;

public class RussianWordsProvider
{
    private readonly string[] words = File.ReadAllLines($"dictionaries{Path.DirectorySeparatorChar}most_used_russian_words");

    public IEnumerable<string> GetWords => words;
}