namespace FormatParser.Test.Helpers;

public static class ShellRunner
{
    public static string RunCommand(string command) => RunCommand(command, "");
    public static string RunCommand(string command, string args)
    {
        var process = new System.Diagnostics.Process();
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            FileName = command,
            Arguments = args ,
            RedirectStandardOutput = true,
        };
        process.StartInfo = startInfo;
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }
}