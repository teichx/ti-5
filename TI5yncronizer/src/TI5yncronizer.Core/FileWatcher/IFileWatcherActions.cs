namespace TI5yncronizer.Core.FileWatcher;

public interface IFileWatcherActions
{
    Task OnChanged(FileSystemEventArgs e, IWatcher watcher);
    Task OnCreated(FileSystemEventArgs e, IWatcher watcher);
    Task OnDeleted(FileSystemEventArgs e, IWatcher watcher);
    Task OnRenamed(RenamedEventArgs e, IWatcher watcher);
    Task OnError(ErrorEventArgs e, IWatcher watcher);
}
