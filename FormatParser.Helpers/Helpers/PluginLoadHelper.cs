using System.Reflection;

namespace FormatParser.Helpers;

public static class PluginLoadHelper
{
    public static void LoadPlugins()
    {
        var pluginDirectory = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}";

        if (!Directory.Exists(pluginDirectory))
            return;

        foreach (var directory in Directory.GetDirectories(pluginDirectory))
        {
            var plugins = Directory
                .EnumerateFiles(directory)
                .Where(x => x.EndsWith(".dll"));

            foreach (var dll in plugins)
            {
                try
                {
                    var a = Assembly.LoadFrom(dll);

                    if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName == a.FullName))
                        continue;

                    AppDomain.CurrentDomain.Load(a.GetName());
                }
                catch (FileLoadException)
                {
                }
                catch (BadImageFormatException)
                {
                }
            }
        }
    }
}