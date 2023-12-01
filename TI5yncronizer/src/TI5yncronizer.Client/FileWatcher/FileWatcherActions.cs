using TI5yncronizer.Core.Extensions;
using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.FileWatcher;

public class FileWatcherActions(ILogger<FileWatcherActions> logger) : IFileWatcherActions
{
    object __locker = new();
    protected virtual bool OriginIsLocal { get; } = true;

    internal Task HandleCopy(string fullPath, IWatcher watcher, bool deleteOld = false)
    {
        if (FileExtension.ShouldIgnoreSync(fullPath)) return Task.CompletedTask;
        var origin = fullPath.TrimStart('\\');

        var fileName = Path.GetFileName(origin);
        var basePath = OriginIsLocal ? watcher.ServerPath : watcher.LocalPath;
        var finalDestiny = Path.Combine(basePath, fileName);

        logger.LogDebug("Copy \nFrom '{}' \nTo '{}' when delete old is {}", origin, finalDestiny, deleteOld);

        if (File.Exists(finalDestiny) is false)
        {
            File.Copy(origin, finalDestiny);
            return Task.CompletedTask;
        }
        if (deleteOld is false) return Task.CompletedTask;

        if (FileExtension.IsSameFiles(origin, finalDestiny)) return Task.CompletedTask;
        var tempFileName = Guid.NewGuid().ToString() + ".tmp";
        var tempDestiny = Path.Combine(basePath, tempFileName);

        var secondaryTempFileName = Guid.NewGuid().ToString() + ".tmp";
        var secondaryTempDestiny = Path.Combine(basePath, secondaryTempFileName);

        File.Copy(origin, tempDestiny);

        File.Move(finalDestiny, secondaryTempDestiny);
        File.Move(tempDestiny, finalDestiny);

        File.Delete(tempDestiny);
        File.Delete(secondaryTempDestiny);

        return Task.CompletedTask;
    }

    public Task OnChanged(FileSystemEventArgs e, IWatcher watcher)
    {
        lock (__locker)
        {
            return HandleCopy(e.FullPath, watcher, deleteOld: true);
        }
    }

    public Task OnCreated(FileSystemEventArgs e, IWatcher watcher)
    {
        lock (__locker)
        {
            return HandleCopy(e.FullPath, watcher);
        }
    }

    public Task OnDeleted(FileSystemEventArgs e, IWatcher watcher)
    {
        if (FileExtension.ShouldIgnoreSync(e.FullPath)) return Task.CompletedTask;

        lock (__locker)
        {
            var fileName = Path.GetFileName(e.FullPath);
            var basePath = OriginIsLocal ? watcher.ServerPath : watcher.LocalPath;
            var destiny = Path.Combine(basePath, fileName);
            logger.LogDebug("OnDeleted local {} to delete server {}", e.FullPath, destiny);
            if (File.Exists(destiny) is false)
            {
                logger.LogWarning("Deleted file with destiny '{}' not exists", destiny);
                return Task.CompletedTask;
            }
            File.Delete(destiny);

            return Task.CompletedTask;
        }
    }

    public Task OnError(ErrorEventArgs e, IWatcher watcher)
    {
        logger.LogError(e.GetException(), "Error on file watcher");
        return Task.CompletedTask;
    }

    public Task OnRenamed(RenamedEventArgs e, IWatcher watcher)
    {
        if (FileExtension.ShouldIgnoreSync(e.FullPath)) return Task.CompletedTask;
        lock (__locker)
        {
            var oldFileName = Path.GetFileName(e.OldFullPath);
            var fileName = Path.GetFileName(e.FullPath);

            var basePath = OriginIsLocal ? watcher.ServerPath : watcher.LocalPath;
            var origin = Path.Combine(basePath, oldFileName);
            var destiny = Path.Combine(basePath, fileName);
            logger.LogDebug("OnRenamed {} to change to {}", origin, destiny);
            if (File.Exists(origin) is false)
            {
                logger.LogWarning("Renamed file origin '{}' not exists", origin);
                return Task.CompletedTask;
            }
            if (File.Exists(destiny))
            {
                logger.LogWarning("Renamed file destiny '{}' already exists", destiny);
                return Task.CompletedTask;
            }
            File.Move(origin, destiny);

            return Task.CompletedTask;
        }
    }
}
