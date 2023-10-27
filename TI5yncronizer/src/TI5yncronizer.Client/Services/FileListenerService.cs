using Grpc.Core;
using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.Services;

public class FileListenerService(IFileWatcher fileWatcher) : FileListener.FileListenerBase
{
    public override Task<AddListenerReply> AddListener(AddListenerRequest request, ServerCallContext context)
    {
        var status = fileWatcher.AddWatcher(request.Folder);
        return Task.FromResult(new AddListenerReply
        {
            Status = (int)status,
        });
    }

    public override Task<RemoveListenerReply> RemoveListener(RemoveListenerRequest request, ServerCallContext context)
    {
        var status = fileWatcher.RemoveWatcher(request.Folder);
        return Task.FromResult(new RemoveListenerReply
        {
            Status = (int)status,
        });
    }

    public override Task<ListListenersReply> ListListeners(ListListenersRequest request, ServerCallContext context)
    {
        var result = new ListListenersReply();
        result.Listeners.AddRange(fileWatcher.FolderWatched.Select(x => new ListListenersObject
        {
            Folder = x,
        }));

        return Task.FromResult(result);
    }
}
