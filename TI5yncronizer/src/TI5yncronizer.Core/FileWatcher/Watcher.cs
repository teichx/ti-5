namespace TI5yncronizer.Core.FileWatcher;

public readonly struct Watcher : IWatcher
{
    readonly string localPath;
    public readonly string LocalPath { get => localPath; init => localPath = PathNormalizer(value); }

    readonly string serverPath;
    public readonly string ServerPath { get => serverPath; init => serverPath = PathNormalizer(value); }
    public readonly string DeviceIdentifier { get; init; }

    public bool IsValid()
    {
        string[] requiredItems = [
            LocalPath,
            ServerPath,
            DeviceIdentifier,
        ];
        return !requiredItems.Any(string.IsNullOrEmpty);
    }

    public override int GetHashCode()
        => HashCode.Combine(LocalPath, ServerPath, DeviceIdentifier);

    public static string PathNormalizer(string path)
        => path
            .Replace('/', '\\')
            .TrimStart('\\')
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
}
