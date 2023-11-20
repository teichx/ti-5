using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;

namespace TI5yncronizer.Server.Helpers;

public static class Initializer
{
    public static async ValueTask Init(this IServiceProvider serviceProvider)
    {
        using var dbContext = serviceProvider.GetRequiredService<DataContext>();
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

        listenersPaths.ForEach(x => fileWatcher.AddWatcher(x.LocalPath));
    }
}
