using TI5yncronizer.Core;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;
using TI5yncronizer.Server.Model;

namespace TI5yncronizer.Server.FileWatcher;

public class FileWatcherActions(
    ILogger<FileWatcherActions> logger,
    DataContext dataContext
) : IFileWatcherActions
{
    List<string> ListDeviceIdentifierListAsync(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath)
            ?.Replace('\\', '/');

        return dataContext.Listener
            .Where(x => x.ServerPath == directory)
            .Select(x => x.DeviceIdentifier)
            .ToList();
    }

    public void OnChanged(FileSystemEventArgs e, IWatcher watcher)
    {
        var pendingSynchronizeItems = ListDeviceIdentifierListAsync(e.FullPath)
            .Select(deviceIdentifier => new PendingSynchronizeModel
            {
                Action = EnumAction.Changed,
                LocalPath = e.FullPath,
                DeviceIdentifier = deviceIdentifier,
            });

        dataContext.PendingSynchronizer.AddRange(pendingSynchronizeItems);
        dataContext.SaveChanges();
    }

    public void OnCreated(FileSystemEventArgs e, IWatcher watcher)
    {
        var pendingSynchronizeItems = ListDeviceIdentifierListAsync(e.FullPath)
            .Select(deviceIdentifier => new PendingSynchronizeModel
            {
                Action = EnumAction.Created,
                LocalPath = e.FullPath,
                DeviceIdentifier = deviceIdentifier,
            });

        dataContext.PendingSynchronizer.AddRange(pendingSynchronizeItems);
        dataContext.SaveChanges();
    }

    public void OnDeleted(FileSystemEventArgs e, IWatcher watcher)
    {
        var pendingSynchronizeItems = ListDeviceIdentifierListAsync(e.FullPath)
            .Select(deviceIdentifier => new PendingSynchronizeModel
            {
                Action = EnumAction.Deleted,
                LocalPath = e.FullPath,
                DeviceIdentifier = deviceIdentifier,
            });

        dataContext.PendingSynchronizer.AddRange(pendingSynchronizeItems);
        dataContext.SaveChanges();
    }

    public void OnError(ErrorEventArgs e, IWatcher watcher)
        => logger.LogError(e.GetException(), "OnError");

    public void OnRenamed(RenamedEventArgs e, IWatcher watcher)
    {
        var pendingSynchronizeItems = ListDeviceIdentifierListAsync(e.FullPath)
            .Select(deviceIdentifier => new PendingSynchronizeModel
            {
                Action = EnumAction.Renamed,
                LocalPath = e.FullPath,
                OldLocalPath = e.OldFullPath,
                DeviceIdentifier = deviceIdentifier,
            });

        dataContext.PendingSynchronizer.AddRange(pendingSynchronizeItems);
        dataContext.SaveChanges();
    }
}
