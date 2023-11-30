using TI5yncronizer.Core.Extensions;
using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.FileWatcher;

public class FileWatcherActions(ILogger<FileWatcherActions> logger) : IFileWatcherActions
{
    internal Task HandleCopy(string fullPath, IWatcher watcher, bool deleteOld = false)
    {
        var origin = fullPath.TrimStart('\\');

        var fileName = Path.GetFileName(origin);
        var finalDestiny = Path.Combine(watcher.ServerPath, fileName);

        logger.LogDebug("Copy \nFrom '{}' \nTo '{}'", origin, finalDestiny);

        if (File.Exists(finalDestiny) is false)
        {
            File.Copy(origin, finalDestiny);
            return Task.CompletedTask;
        }
        if (deleteOld is false) throw new IOException($"The file '{finalDestiny}' already exists");

        if (FileExtension.IsSameFiles(origin, finalDestiny)) return Task.CompletedTask;
        var tempFileName = Guid.NewGuid().ToString() + ".tmp";
        var tempDestiny = Path.Combine(watcher.ServerPath, tempFileName);

        var secondaryTempFileName = Guid.NewGuid().ToString() + ".tmp";
        var secondaryTempDestiny = Path.Combine(watcher.ServerPath, secondaryTempFileName);

        File.Copy(origin, tempDestiny);

        File.Move(finalDestiny, secondaryTempDestiny);
        File.Move(tempDestiny, finalDestiny);

        File.Delete(tempDestiny);
        File.Delete(secondaryTempDestiny);

        return Task.CompletedTask;
    }

    public Task OnChanged(FileSystemEventArgs e, IWatcher watcher)
        => HandleCopy(e.FullPath, watcher, deleteOld: true);

    public Task OnCreated(FileSystemEventArgs e, IWatcher watcher)
        => HandleCopy(e.FullPath, watcher);

    public Task OnDeleted(FileSystemEventArgs e, IWatcher watcher)
    {
        var fileName = Path.GetFileName(e.FullPath);
        var destiny = Path.Combine(watcher.ServerPath, fileName);
        logger.LogDebug("OnDeleted local {} to delete server {}", e.FullPath, destiny);
        if (File.Exists(destiny) is false)
        {
            logger.LogWarning("Deleted file with destiny '{}' not exists", destiny);
            return Task.CompletedTask;
        }
        File.Delete(destiny);

        return Task.CompletedTask;
    }

    public Task OnError(ErrorEventArgs e, IWatcher watcher)
    {
        logger.LogError(e.GetException(), "Error on file watcher");
        return Task.CompletedTask;
    }

    public Task OnRenamed(RenamedEventArgs e, IWatcher watcher)
    {
        var oldFileName = Path.GetFileName(e.OldFullPath);
        var fileName = Path.GetFileName(e.FullPath);
        var origin = Path.Combine(watcher.ServerPath, oldFileName);
        var destiny = Path.Combine(watcher.ServerPath, fileName);
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
