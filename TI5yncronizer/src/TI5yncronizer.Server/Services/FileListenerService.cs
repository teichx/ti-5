using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;
using TI5yncronizer.Server.Model;

namespace TI5yncronizer.Server.Services;

public class FileListenerService(
    IFileWatcher fileWatcher,
    IDbContextFactory<DataContext> dbContextFactory,
    ILogger<FileListenerService> logger
) : FileListener.FileListenerBase
{
    DataContext dataContext = dbContextFactory.CreateDbContext();

    public override async Task<AddListenerReply> AddListener(AddListenerRequest request, ServerCallContext context)
    {
        var watcher = new Watcher
        {
            LocalPath = request.LocalPath,
            ServerPath = request.ServerPath,
            DeviceIdentifier = request.DeviceIdentifier,
        };
        if (watcher.IsValid() is false)
        {
            return new AddListenerReply
            {
                Status = (int)EnumFileWatcher.TryCreateInvalid,
            };
        }
        var listenerModel = await dataContext.Listener
            .Where(x => x.LocalPath == watcher.LocalPath)
            .Where(x => x.DeviceIdentifier == watcher.DeviceIdentifier)
            .Where(x => x.ServerPath == watcher.ServerPath)
            .SingleOrDefaultAsync()
            ?? ListenerModel.FromWatcher(watcher);

        if (listenerModel.Id is 0)
        {
            await dataContext.Listener.AddAsync(listenerModel);
        }
        if (listenerModel.DeletedAt is not null)
        {
            listenerModel.DeletedAt = null;
        }

        listenerModel.UpdatedAt = DateTime.UtcNow;
        var status = fileWatcher.AddWatcher(watcher);
        await dataContext.SaveChangesAsync();
        await fileWatcher.NotifyChangeRecursive(watcher);
        return new AddListenerReply
        {
            Id = listenerModel.Id,
            Status = (int)status,
        };
    }

    public override async Task<RemoveListenerReply> RemoveListener(RemoveListenerRequest request, ServerCallContext context)
    {
        var listener = await dataContext.Listener
            .SingleOrDefaultAsync(x => x.Id == request.Id);
        if (listener is null)
        {
            return new RemoveListenerReply
            {
                Status = (int)EnumFileWatcher.TryRemoveNotExists,
            };
        }

        var status = fileWatcher.RemoveWatcher(listener.ToWatcher());
        await dataContext.Listener
            .Where(x => x.Id == request.Id)
            .Where(x => x.DeletedAt == null)
            .ExecuteUpdateAsync(x => x
                .SetProperty(y => y.DeletedAt, DateTime.UtcNow)
                .SetProperty(y => y.UpdatedAt, DateTime.UtcNow)
            );

        return new RemoveListenerReply
        {
            Status = (int)status,
        };
    }

    IQueryable<ListenerModel> FilterByUpdatedAfter(IQueryable<ListenerModel> query, string? updatedAfter)
    {
        if (string.IsNullOrWhiteSpace(updatedAfter)) return query;

        var createdAfterParsed = DateTime.Parse(updatedAfter);
        return query.Where(x => x.UpdatedAt >= createdAfterParsed);
    }

    public override async Task<ListListenersReply> ListListeners(ListListenersRequest request, ServerCallContext context)
    {
        var query = dataContext.Listener
            .Where(x => x.DeviceIdentifier == request.DeviceIdentifier);

        var filteredByDate = FilterByUpdatedAfter(query, request.UpdatedAfter);

        var listenerList = await filteredByDate
            .Select(x => new
            {
                x.LocalPath,
                x.ServerPath,
                x.DeletedAt,
            })
            .ToListAsync();

        var result = new ListListenersReply();
        result.Listeners.AddRange(listenerList?.Select(x => new ListListenersObject
        {
            LocalPath = x.LocalPath,
            ServerPath = x.ServerPath,
            IsActive = x.DeletedAt is null,
        }));

        return result;
    }

    public override async Task<ListPendingFilesToSyncReply> ListPendingFilesToSync(ListPendingFilesToSyncRequest request, ServerCallContext context)
    {
        var fileList = await dataContext.PendingSynchronizer
            .Where(x => x.DeviceIdentifier == request.DeviceIdentifier)
            .Select(x => new
            {
                x.Id,
                x.LocalPath,
                x.ServerPath,
                x.OldServerPath,
                x.Action,
                x.LastWriteUtcAsTicks,
            })
            .Take(100)
            .ToListAsync();

        var result = new ListPendingFilesToSyncReply();
        result.Files.AddRange(fileList?.Select(x => new ListPendingFilesToSyncObject
        {
            Id = x.Id,
            LocalPath = Watcher.PathNormalizer(x.LocalPath),
            ServerPath = Watcher.PathNormalizer(x.ServerPath),
            OldServerPath = Watcher.PathNormalizer(x.OldServerPath ?? string.Empty),
            EnumAction = (int)x.Action,
        }));

        return result;
    }

    public override async Task<FileSynchronizedReply> FileSynchronized(FileSynchronizedRequest request, ServerCallContext context)
    {
        var itemsUpdated = await dataContext.PendingSynchronizer
            .Where(x => x.Id == request.Id)
            .Where(x => x.DeviceIdentifier == request.DeviceIdentifier)
            .ExecuteDeleteAsync();

        if (itemsUpdated is 0)
        {
            logger.LogError("Failed on delete registers for id={Id} and device={DeviceIdentifier}", request.Id, request.DeviceIdentifier);
        }

        return new FileSynchronizedReply();
    }
}
