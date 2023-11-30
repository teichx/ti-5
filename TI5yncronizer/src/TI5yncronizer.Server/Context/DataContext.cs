using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Server.Model;

namespace TI5yncronizer.Server.Context;

public class DataContext(IConfiguration configuration, ILogger<DataContext> logger) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlite(configuration.GetConnectionString("SQLite"))
            .LogTo(logMessage => logger.LogDebug(logMessage))
            .EnableDetailedErrors(Debugger.IsAttached)
            .EnableSensitiveDataLogging(Debugger.IsAttached)
            .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

    public DbSet<ListenerModel> Listener { get; set; }
    public DbSet<PendingSynchronizeModel> PendingSynchronizer { get; set; }
}
