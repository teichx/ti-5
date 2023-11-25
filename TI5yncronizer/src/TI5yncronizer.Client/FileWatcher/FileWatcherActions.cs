using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.FileWatcher;

public class FileWatcherActions(ILogger<FileWatcherActions> logger) : IFileWatcherActions
{
    public void OnChanged(FileSystemEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnChanged {FullPath}", e.FullPath);
    }

    public void OnCreated(FileSystemEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnCreated {FullPath}", e.FullPath);
    }

    public void OnDeleted(FileSystemEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnDeleted {FullPath}", e.FullPath);
    }

    public void OnError(ErrorEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnError {Error}", e);
    }

    public void OnRenamed(RenamedEventArgs e, IWatcher watcher)
    {
        logger.LogInformation("OnRenamed {FullPath}", e.FullPath);
    }
}
