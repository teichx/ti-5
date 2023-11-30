using Grpc.Net.Client;
using TI5yncronizer.Core;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Core.ValueObject;

namespace TI5yncronizer.Client.Background;

public class ListenerHostedService(
    ILogger<ListenerHostedService> logger,
    IConfiguration configuration,
    IFileWatcher fileWatcher
) : IHostedService, IDisposable
{
    bool _inRequest;
    DateTime? _lastQuery;
    Timer? _timer;
    GrpcChannel? _grpcChannel;
    FileListener.FileListenerClient? _fileListenerClient;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start");
        _grpcChannel = GrpcChannel.ForAddress(configuration.GetConnectionString("Server")!);
        _fileListenerClient = new FileListener.FileListenerClient(_grpcChannel);
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    void DoWork(object? state)
    {
        ArgumentNullException.ThrowIfNull(_fileListenerClient);
        if (_inRequest) return;
        try
        {
            _inRequest = true;
            var queryInit = DateTime.UtcNow;
            var listeners = _fileListenerClient.ListListeners(new ListListenersRequest
            {
                UpdatedAfter = _lastQuery is null ? string.Empty : _lastQuery.ToString(),
                DeviceIdentifier = Device.DefaultDevice.Value,
            });
            foreach (var item in listeners.Listeners)
            {
                var watcher = new Watcher
                {
                    LocalPath = item.LocalPath,
                    ServerPath = item.ServerPath,
                    DeviceIdentifier = Device.DefaultDevice.Value,
                };

                if (item.IsActive)
                {
                    fileWatcher.AddWatcher(watcher);
                    fileWatcher.NotifyChangeRecursive(watcher).GetAwaiter().GetResult();
                    continue;
                }

                fileWatcher.RemoveWatcher(watcher);
            }

            _lastQuery = queryInit;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed on get listeners data");
        }
        finally
        {
            _inRequest = false;
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer?.Dispose();
        _grpcChannel?.Dispose();
    }
}
