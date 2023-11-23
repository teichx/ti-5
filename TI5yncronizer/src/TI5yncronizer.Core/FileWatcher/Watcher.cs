namespace TI5yncronizer.Core.FileWatcher;

public readonly struct Watcher : IWatcher
{
    public readonly string LocalPath { get; init; }
    public readonly string ServerPath { get; init; }
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
}
