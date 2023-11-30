using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Client.FileWatcher;

public interface IFromServerFileWatcherActions : IFileWatcherActions
{

}

public class FromServerFileWatcherActions(ILogger<FileWatcherActions> logger)
    : FileWatcherActions(logger),
        IFromServerFileWatcherActions
{
    protected override bool OriginIsLocal { get; } = false;
}
