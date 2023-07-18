namespace FormatParser.Helpers;

public static class FileInfoExtensions
{
    public static bool IsSymlink(this FileInfo fileInfo) => fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
    public static bool IsEmpty(this FileInfo fileInfo) => fileInfo.Length == 0;
}