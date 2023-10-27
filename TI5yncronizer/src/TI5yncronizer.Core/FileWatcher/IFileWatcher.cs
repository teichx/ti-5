namespace TI5yncronizer.Core.FileWatcher;

public interface IFileWatcher : IDisposable
{
    IReadOnlySet<string> FolderWatched { get; }
    EnumFileWatcher AddWatcher(string path);
    EnumFileWatcher RemoveWatcher(string path);
}
