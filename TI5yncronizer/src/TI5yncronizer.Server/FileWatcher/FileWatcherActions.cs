using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;
using TI5yncronizer.Server.Model;

namespace TI5yncronizer.Server.FileWatcher;

public class FileWatcherActions(
    ILogger<FileWatcherActions> logger,
    IDbContextFactory<DataContext> dbContextFactory
) : IFileWatcherActions
{
    DataContext dataContext = dbContextFactory.CreateDbContext();

    async ValueTask<List<string>> ListDeviceIdentifierListAsync(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath)
            ?.Replace('\\', '/');

        return await dataContext.Listener
            .Where(x => x.ServerPath == directory)
            .Where(x => x.DeletedAt == null)
            .Select(x => x.DeviceIdentifier)
            .ToListAsync();
    }

    public void OnChanged(FileSystemEventArgs e, IWatcher watcher)
        => Task.Run(async () =>
        {
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
            .Select(deviceIdentifier => new PendingSynchronizeModel
            {
                Action = EnumAction.Changed,
                LocalPath = e.FullPath,
                DeviceIdentifier = deviceIdentifier,
            });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });

    public void OnCreated(FileSystemEventArgs e, IWatcher watcher)
        => Task.Run(async () =>
        {
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
                .Select(deviceIdentifier => new PendingSynchronizeModel
                {
                    Action = EnumAction.Created,
                    LocalPath = e.FullPath,
                    DeviceIdentifier = deviceIdentifier,
                });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });

    public void OnDeleted(FileSystemEventArgs e, IWatcher watcher)
        => Task.Run(async () =>
        {
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
                .Select(deviceIdentifier => new PendingSynchronizeModel
                {
                    Action = EnumAction.Deleted,
                    LocalPath = e.FullPath,
                    DeviceIdentifier = deviceIdentifier,
                });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });

    public void OnError(ErrorEventArgs e, IWatcher watcher)
        => logger.LogError(e.GetException(), "OnError");

    public void OnRenamed(RenamedEventArgs e, IWatcher watcher)
        => Task.Run(async () =>
        {
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
                .Select(deviceIdentifier => new PendingSynchronizeModel
                {
                    Action = EnumAction.Renamed,
                    LocalPath = e.FullPath,
                    OldLocalPath = e.OldFullPath,
                    DeviceIdentifier = deviceIdentifier,
                });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });
}
