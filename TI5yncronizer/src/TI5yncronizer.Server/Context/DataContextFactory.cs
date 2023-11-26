using Microsoft.EntityFrameworkCore;

namespace TI5yncronizer.Server.Context;

public class DataContextFactory(IConfiguration configuration, ILoggerFactory loggerFactory) : IDbContextFactory<DataContext>
{
    public virtual DataContext CreateDbContext()
        => new(configuration, loggerFactory.CreateLogger<DataContext>());
}
