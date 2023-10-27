namespace TI5yncronizer.Core.FileWatcher;

public interface IFileWatcherActions
{
    void OnChanged(object sender, FileSystemEventArgs e);
    void OnCreated(object sender, FileSystemEventArgs e);
    void OnDeleted(object sender, FileSystemEventArgs e);
    void OnRenamed(object sender, RenamedEventArgs e);
    void OnError(object sender, ErrorEventArgs e);
}
