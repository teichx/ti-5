namespace TI5yncronizer.Core.Extensions;

public static class FileExtension
{
    public static long GetLastWriteTicks(this string fullPath)
        => File.GetLastWriteTimeUtc(fullPath.TrimStart('\\')).Ticks;
}
