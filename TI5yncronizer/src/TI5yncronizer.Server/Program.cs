using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Server.Context;
using TI5yncronizer.Server.FileWatcher;
using TI5yncronizer.Server.Helpers;
using TI5yncronizer.Server.Services;

Environment.SetEnvironmentVariable("IS_SERVER", "true");
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextFactory<DataContext, DataContextFactory>()
    .AddSingleton<IFileWatcherActions, FileWatcherActions>()
    .AddSingleton<IFileWatcher, FileWatcher>()
    .AddGrpc();

var app = builder.Build();

await app.Services.Init();

// Configure the HTTP request pipeline.
app.MapGrpcService<FileListenerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
