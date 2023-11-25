using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;
using TI5yncronizer.Server.Model;

namespace TI5yncronizer.Server.Services;

public class FileListenerService(IFileWatcher fileWatcher, DataContext dataContext) : FileListener.FileListenerBase
{
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
        var listenerModel = ListenerModel.FromWatcher(watcher);

        var transaction = await dataContext.Database.BeginTransactionAsync();
        try
        {
            var listener = await dataContext.Listener.AddAsync(listenerModel);
            await dataContext.SaveChangesAsync();
            var status = fileWatcher.AddWatcher(watcher);
            await transaction.CommitAsync();

            return new AddListenerReply
            {
                Id = listener.Entity.Id,
                Status = (int)status,
            };
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            var listenerFromDb = await dataContext.Listener
                .Where(x => x.DeviceIdentifier == listenerModel.DeviceIdentifier)
                .Where(x => x.LocalPath == listenerModel.LocalPath)
                .Where(x => x.ServerPath == listenerModel.ServerPath)
                .SingleAsync();
            return new AddListenerReply
            {
                Id = listenerFromDb.Id,
                Status = (int)EnumFileWatcher.TryCreateAlreadyExists,
            };
        }
        catch
        {
            fileWatcher.RemoveWatcher(watcher);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public override async Task<RemoveListenerReply> RemoveListener(RemoveListenerRequest request, ServerCallContext context)
    {
        var listener = await dataContext.Listener.SingleOrDefaultAsync(x => x.Id == request.Id);
        if (listener is null)
        {
            return new RemoveListenerReply
            {
                Status = (int)EnumFileWatcher.TryRemoveNotExists,
            };
        }

        var status = fileWatcher.RemoveWatcher(listener.ToWatcher());

        return new RemoveListenerReply
        {
            Status = (int)status,
        };
    }

    IQueryable<ListenerModel> FilterByCreatedAfter(IQueryable<ListenerModel> query, string? createdAfter)
    {
        if (string.IsNullOrWhiteSpace(createdAfter)) return query;

        var createdAfterParsed = DateTime.Parse(createdAfter);
        return query.Where(x => x.CreatedAt >= createdAfterParsed);
    }

    public override async Task<ListListenersReply> ListListeners(ListListenersRequest request, ServerCallContext context)
    {
        var query = dataContext.Listener
            .Where(x => x.DeviceIdentifier == request.DeviceIdentifier);

        var filteredByDate = FilterByCreatedAfter(query, request.CreatedAfter);

        var listenerList = await filteredByDate
            .Select(x => new
            {
                x.LocalPath,
                x.ServerPath,
            })
            .ToListAsync();

        var result = new ListListenersReply();
        result.Listeners.AddRange(listenerList?.Select(x => new ListListenersObject
        {
            LocalPath = x.LocalPath,
            ServerPath = x.ServerPath,
        }));

        return result;
    }
}
