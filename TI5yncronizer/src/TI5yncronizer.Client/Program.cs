using TI5yncronizer.Client.Authentication;
using TI5yncronizer.Client.FileWatcher;
using TI5yncronizer.Client.Services;
using TI5yncronizer.Core.FileWatcher;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAuthSession, AuthSession>();
builder.Services.AddSingleton<IFileWatcherActions, FileWatcherActions>();
builder.Services.AddSingleton<IFileWatcher, FileWatcher>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthService>();
app.MapGrpcService<FileListenerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
