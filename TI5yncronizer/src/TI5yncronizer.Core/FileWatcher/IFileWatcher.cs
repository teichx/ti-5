namespace TI5yncronizer.Core.FileWatcher;

public interface IFileWatcher : IDisposable
{
    EnumFileWatcher AddWatcher(IWatcher watcher);
    EnumFileWatcher RemoveWatcher(IWatcher watcher);
    ValueTask NotifyChangeRecursive(IWatcher watcher);
}
