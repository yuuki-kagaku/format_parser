namespace ConsoleApp1;

public class QualityCheckerState
{
    public int TextFilesAccordingToFileCommand { get; set; } = 0;
    public int TextFilesAccordingToFormatParser { get; set; } = 0;

    public int FalseNegativesOfFileCommand { get; set; } = 0;
    public int FalsePositivesOfFileCommand { get; set; } = 0;
    public int MatchMismatches { get; set; } = 0;
}