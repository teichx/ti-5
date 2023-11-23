namespace TI5yncronizer.Core.FileWatcher;

public enum EnumFileWatcher
{
    TryCreateSuccess = 1,
    TryCreateAlreadyExists,
    TryCreateInvalid,

    TryRemoveSuccess,
    TryRemoveNotExists,
}
