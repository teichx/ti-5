using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace TI5yncronizer.Core.FileWatcher;

public class FileWatcher(
    ILogger<FileWatcher> logger,
    IFileWatcherActions watcherActions
) : IFileWatcher
{
    private readonly ConcurrentDictionary<IWatcher, FileSystemWatcher> watchers = new();

    static string PathFromIWatcher(IWatcher watcher)
        => Environment.GetEnvironmentVariable("IS_SERVER") == "true"
            ? watcher.ServerPath
            : watcher.LocalPath;

    public async ValueTask NotifyChangeRecursive(IWatcher watcher)
    {
        var path = Watcher.PathNormalizer(PathFromIWatcher(watcher));
        foreach (var filePath in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
        {
            var eventChange = new FileSystemEventArgs(
                changeType: WatcherChangeTypes.Changed,
                directory: string.Empty,
                name: filePath
            );
            await watcherActions.OnChanged(eventChange, watcher);
        }
    }

    private FileSystemWatcher CreateWatcher(IWatcher watcher)
    {
        var path = PathFromIWatcher(watcher);
        var fileWatcher = new FileSystemWatcher(path)
        {
            NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size
        };

        fileWatcher.Changed += (a, b) => watcherActions.OnChanged(b, watcher);
        fileWatcher.Created += (a, b) => watcherActions.OnCreated(b, watcher);
        fileWatcher.Deleted += (a, b) => watcherActions.OnDeleted(b, watcher);
        fileWatcher.Renamed += (a, b) => watcherActions.OnRenamed(b, watcher);
        fileWatcher.Error += (a, b) => watcherActions.OnError(b, watcher);

        fileWatcher.IncludeSubdirectories = true;
        fileWatcher.EnableRaisingEvents = true;

        return fileWatcher;
    }

    public EnumFileWatcher AddWatcher(IWatcher watcher)
    {
        logger.LogInformation("Try add watcher for LocalPath='{LocalPath}' ServerPath='{ServerPath}' DeviceIdentifier={DeviceIdentifier}", watcher.LocalPath, watcher.ServerPath, watcher.DeviceIdentifier);
        if (watchers.TryAdd(watcher, CreateWatcher(watcher)))
            return EnumFileWatcher.TryCreateSuccess;

        return EnumFileWatcher.TryCreateAlreadyExists;
    }

    public EnumFileWatcher RemoveWatcher(IWatcher watcher)
    {
        logger.LogInformation("Try remove watcher for LocalPath={LocalPath} ServerPath={ServerPath} DeviceIdentifier={DeviceIdentifier}", watcher.LocalPath, watcher.ServerPath, watcher.DeviceIdentifier);
        if (watchers.TryRemove(watcher, out var fileSystemWatcher))
        {
            fileSystemWatcher.Dispose();
            return EnumFileWatcher.TryRemoveSuccess;
        }

        return EnumFileWatcher.TryRemoveNotExists;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var item in watchers)
        {
            item.Value.Dispose();
            watchers.TryRemove(item);
        }
    }
}
