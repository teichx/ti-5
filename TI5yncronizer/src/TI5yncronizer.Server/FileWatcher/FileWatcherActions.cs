using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core;
using TI5yncronizer.Core.Extensions;
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
        var directory = Watcher.PathNormalizer(Path.GetDirectoryName(fullPath) ?? string.Empty);

        return await dataContext.Listener
            .Where(x => directory.Contains(x.ServerPath) || directory.Contains(x.LocalPath))
            .Where(x => x.DeletedAt == null)
            .Select(x => x.DeviceIdentifier)
            .Distinct()
            .ToListAsync();
    }

    async Task PreventDuplicationEvent(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (DbUpdateException) { }
        catch (Exception e)
        {
            logger.LogError(e, "Failed on run file");
        }
    }

    public async Task OnChanged(FileSystemEventArgs e, IWatcher watcher)
        => await PreventDuplicationEvent(async () =>
        {
            if (FileExtension.ShouldIgnoreSync(e.FullPath)) return;
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
            .Select(deviceIdentifier => new PendingSynchronizeModel
            {
                Action = EnumAction.Changed,
                LocalPath = watcher.LocalPath,
                ServerPath = e.FullPath,
                DeviceIdentifier = deviceIdentifier,
                LastWriteUtcAsTicks = e.FullPath.GetLastWriteTicks(),
            });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });

    public async Task OnCreated(FileSystemEventArgs e, IWatcher watcher)
        => await PreventDuplicationEvent(async () =>
        {
            if (FileExtension.ShouldIgnoreSync(e.FullPath)) return;
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
                .Select(deviceIdentifier => new PendingSynchronizeModel
                {
                    Action = EnumAction.Created,
                    LocalPath = watcher.LocalPath,
                    ServerPath = e.FullPath,
                    DeviceIdentifier = deviceIdentifier,
                    LastWriteUtcAsTicks = e.FullPath.GetLastWriteTicks(),
                });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });

    public async Task OnDeleted(FileSystemEventArgs e, IWatcher watcher)
        => await PreventDuplicationEvent(async () =>
        {
            if (FileExtension.ShouldIgnoreSync(e.FullPath)) return;
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
                .Select(deviceIdentifier => new PendingSynchronizeModel
                {
                    Action = EnumAction.Deleted,
                    LocalPath = watcher.LocalPath,
                    ServerPath = e.FullPath,
                    DeviceIdentifier = deviceIdentifier,
                    LastWriteUtcAsTicks = e.FullPath.GetLastWriteTicks(),
                });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });

    public Task OnError(ErrorEventArgs e, IWatcher watcher)
    {
        logger.LogError(e.GetException(), "OnError");
        return Task.CompletedTask;
    }

    public async Task OnRenamed(RenamedEventArgs e, IWatcher watcher)
        => await PreventDuplicationEvent(async () =>
        {
            if (FileExtension.ShouldIgnoreSync(e.FullPath)) return;
            var pendingSynchronizeItems = (await ListDeviceIdentifierListAsync(e.FullPath))
                .Select(deviceIdentifier => new PendingSynchronizeModel
                {
                    Action = EnumAction.Renamed,
                    LocalPath = watcher.LocalPath,
                    ServerPath = e.FullPath,
                    OldServerPath = e.OldFullPath,
                    DeviceIdentifier = deviceIdentifier,
                    LastWriteUtcAsTicks = e.FullPath.GetLastWriteTicks(),
                });

            await dataContext.PendingSynchronizer.AddRangeAsync(pendingSynchronizeItems);
            await dataContext.SaveChangesAsync();
        });
}
