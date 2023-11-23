namespace TI5yncronizer.Core.FileWatcher;

public interface IWatcher
{
    string LocalPath { get; }
    string ServerPath { get; }
    string DeviceIdentifier { get; }
}
