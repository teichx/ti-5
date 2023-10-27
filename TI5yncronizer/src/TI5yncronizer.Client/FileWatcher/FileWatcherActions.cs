using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.FileWatcher;

public class FileWatcherActions(ILogger<FileWatcherActions> logger) : IFileWatcherActions
{
    public void OnChanged(object sender, FileSystemEventArgs e)
    {
        logger.LogInformation("OnChanged {FullPath}", e.FullPath);
    }

    public void OnCreated(object sender, FileSystemEventArgs e)
    {
        logger.LogInformation("OnCreated {FullPath}", e.FullPath);
    }

    public void OnDeleted(object sender, FileSystemEventArgs e)
    {
        logger.LogInformation("OnDeleted {FullPath}", e.FullPath);
    }

    public void OnError(object sender, ErrorEventArgs e)
    {
        logger.LogInformation("OnError {Error}", e);
    }

    public void OnRenamed(object sender, RenamedEventArgs e)
    {
        logger.LogInformation("OnRenamed {FullPath}", e.FullPath);
    }
}
