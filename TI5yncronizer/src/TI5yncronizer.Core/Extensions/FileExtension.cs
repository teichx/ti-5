using System.Security.Cryptography;

namespace TI5yncronizer.Core.Extensions;

public static class FileExtension
{
    static readonly MD5 MD5Instance = MD5.Create();

    public static long GetLastWriteTicks(this string fullPath)
        => string.IsNullOrWhiteSpace(fullPath)
            ? 0
            : File.GetLastWriteTimeUtc(fullPath.TrimStart('\\')).Ticks;

    public static string ComputeFileHash(string filePath)
    {
        using var file = File.OpenRead(filePath);
        var hash = MD5Instance.ComputeHash(file);
        return string.Join(string.Empty, hash.Select(x => x.ToString("x2")));
    }

    public static bool IsSameFiles(string leftFile, string rightFile)
    {
        try
        {
            return ComputeFileHash(leftFile) == ComputeFileHash(rightFile);
        }
        catch
        {
            return false;
        }
    }
}
