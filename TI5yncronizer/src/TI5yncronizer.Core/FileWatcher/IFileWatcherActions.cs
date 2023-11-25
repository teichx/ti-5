namespace TI5yncronizer.Core.FileWatcher;

public interface IFileWatcherActions
{
    void OnChanged(FileSystemEventArgs e, IWatcher watcher);
    void OnCreated(FileSystemEventArgs e, IWatcher watcher);
    void OnDeleted(FileSystemEventArgs e, IWatcher watcher);
    void OnRenamed(RenamedEventArgs e, IWatcher watcher);
    void OnError(ErrorEventArgs e, IWatcher watcher);
}
