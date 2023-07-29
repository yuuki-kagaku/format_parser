namespace FormatParser.Helpers;

public static class DirectoryInfoExtensions
{
    public static bool IsSymlink(this DirectoryInfo fileInfo) => fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
}