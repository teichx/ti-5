using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TI5yncronizer.Core.FileWatcher;

public class FileWatcher(
    ILogger<FileWatcher> logger,
    IFileWatcherActions watcherActions
) : IFileWatcher
{
    private readonly ConcurrentDictionary<IWatcher, FileSystemWatcher> watchers = new();

    void NotifyChangeRecursive(string path, IWatcher watcher)
    {
        var directory = Path.GetDirectoryName(path)!;
        foreach (var filePath in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
        {
            var eventChange = new FileSystemEventArgs(
                changeType: WatcherChangeTypes.Changed,
                directory: directory,
                name: filePath
            );
            watcherActions.OnChanged(eventChange, watcher);
        }
    }

    private FileSystemWatcher CreateWatcher(IWatcher watcher)
    {
        var isServer = Environment.GetEnvironmentVariable("IS_SERVER") == "true";
        var path = isServer ? watcher.ServerPath : watcher.LocalPath;
        NotifyChangeRecursive(path, watcher);
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
        logger.LogInformation("Try add watcher for LocalPath={LocalPath} ServerPath={ServerPath} DeviceIdentifier={DeviceIdentifier}", watcher.LocalPath, watcher.ServerPath, watcher.DeviceIdentifier);
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
