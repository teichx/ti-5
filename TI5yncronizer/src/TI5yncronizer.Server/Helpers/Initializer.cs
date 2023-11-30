using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;

namespace TI5yncronizer.Server.Helpers;

public static class Initializer
{
    public static async ValueTask Init(this IServiceProvider serviceProvider)
    {

        var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var fileWatcher = serviceProvider.GetRequiredService<IFileWatcher>();

        await Migrate(dbContext);
        await AddListeners(dbContext, fileWatcher);
    }

    public static async ValueTask Migrate(DataContext dataContext)
        => await dataContext.Database.MigrateAsync();

    public static async ValueTask AddListeners(DataContext dataContext, IFileWatcher fileWatcher)
    {
        var listenersPaths = await dataContext.Listener
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();

        foreach (var item in listenersPaths)
        {
            var watcher = item.ToWatcher();
            fileWatcher.AddWatcher(watcher);
            await fileWatcher.NotifyChangeRecursive(watcher);
        }
    }
}
