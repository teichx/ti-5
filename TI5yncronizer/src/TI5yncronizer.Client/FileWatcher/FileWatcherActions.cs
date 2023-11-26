using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.FileWatcher;

public class FileWatcherActions(ILogger<FileWatcherActions> logger) : IFileWatcherActions
{
    public Task OnChanged(FileSystemEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnChanged {FullPath}", e.FullPath);
        return Task.CompletedTask;
    }

    public Task OnCreated(FileSystemEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnCreated {FullPath}", e.FullPath);
        return Task.CompletedTask;
    }

    public Task OnDeleted(FileSystemEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnDeleted {FullPath}", e.FullPath);
        return Task.CompletedTask;
    }

    public Task OnError(ErrorEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnError {Error}", e);
        return Task.CompletedTask;
    }

    public Task OnRenamed(RenamedEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnRenamed {FullPath}", e.FullPath);
        return Task.CompletedTask;
    }
}
