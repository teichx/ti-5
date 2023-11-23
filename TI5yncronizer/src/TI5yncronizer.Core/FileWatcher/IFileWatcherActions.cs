namespace TI5yncronizer.Core.FileWatcher;

public interface IFileWatcherActions
{
    void OnChanged(object sender, FileSystemEventArgs e, IWatcher watcher);
    void OnCreated(object sender, FileSystemEventArgs e, IWatcher watcher);
    void OnDeleted(object sender, FileSystemEventArgs e, IWatcher watcher);
    void OnRenamed(object sender, RenamedEventArgs e, IWatcher watcher);
    void OnError(object sender, ErrorEventArgs e, IWatcher watcher);
}
