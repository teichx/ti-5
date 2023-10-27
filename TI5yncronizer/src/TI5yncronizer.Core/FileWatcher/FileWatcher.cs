using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace TI5yncronizer.Core.FileWatcher;

public class FileWatcher(ILogger<FileWatcher> logger, IFileWatcherActions watcherActions) : IFileWatcher
{
    private readonly ConcurrentDictionary<string, FileSystemWatcher> watchers = new();
    public IReadOnlySet<string> FolderWatched => watchers
        .Keys
        .ToImmutableHashSet();

    private FileSystemWatcher CreateWatcher(string path)
    {
        var watcher = new FileSystemWatcher(path)
        {
            NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size
        };

        watcher.Changed += watcherActions.OnChanged;
        watcher.Created += watcherActions.OnCreated;
        watcher.Deleted += watcherActions.OnDeleted;
        watcher.Renamed += watcherActions.OnRenamed;
        watcher.Error += watcherActions.OnError;

        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        return watcher;
    }

    public EnumFileWatcher AddWatcher(string path)
    {
        logger.LogInformation("Try create watcher for path \"{Path}\"", path);

        if (watchers.TryAdd(path, CreateWatcher(path)))
        {
            logger.LogInformation("Success on create listener for \"{Path}\"", path);
            return EnumFileWatcher.TryCreateSuccess;
        }

        logger.LogInformation("Listener already exists for \"{Path}\"", path);
        return EnumFileWatcher.TryCreateAlreadyExists;
    }

    public EnumFileWatcher RemoveWatcher(string path)
    {
        logger.LogInformation("Try remove watcher for path \"{Path}\"", path);
        if (watchers.TryRemove(path, out _))
        {
            logger.LogInformation("Success on remove listener for \"{Path}\"", path);
            return EnumFileWatcher.TryRemoveSuccess;
        }

        logger.LogInformation("Listener not exists for \"{Path}\"", path);
        return EnumFileWatcher.TryRemoveNotExists;
    }

    public IReadOnlySet<string> ListFolders()
        => FolderWatched;

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
