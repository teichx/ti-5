using Grpc.Net.Client;
using TI5yncronizer.Core;
using TI5yncronizer.Core.FileWatcher;
using TI5yncronizer.Core.ValueObject;

namespace TI5yncronizer.Client.Background;

public class FileWatcherHostedService(
    ILogger<FileWatcherHostedService> logger,
    IConfiguration configuration
) : IHostedService, IDisposable
{
    bool _inRequest;
    Timer? _timer;
    GrpcChannel? _grpcChannel;
    FileListener.FileListenerClient? _fileListenerClient;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start");
        _grpcChannel = GrpcChannel.ForAddress(configuration.GetConnectionString("Server")!);
        _fileListenerClient = new FileListener.FileListenerClient(_grpcChannel);
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));

        return Task.CompletedTask;
    }

    void DoWork(object? state)
    {
        ArgumentNullException.ThrowIfNull(_fileListenerClient);
        if (_inRequest) return;
        try
        {
            _inRequest = true;
            var reply = _fileListenerClient.ListPendingFilesToSync(new ListPendingFilesToSyncRequest
            {
                DeviceIdentifier = Device.DefaultDevice.Value,
            });
            foreach (var file in reply.Files)
            {
                var processed = ProcessFile(file);
                if (processed is false) continue;

                _fileListenerClient.FileSynchronized(new FileSynchronizedRequest
                {
                    Id = file.Id,
                    DeviceIdentifier = Device.DefaultDevice.Value,
                });
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed on get files to synchronize");
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

    bool ProcessFile(ListPendingFilesToSyncObject file)
    {
        try
        {
            var action = ((EnumAction)file.EnumAction).ToString();
            var lastWriteUtc = new DateTime(file.LastWriteUtcAsTicks);
            logger.LogInformation("Try {Action} from {Server} to {Local} with id {Id} lastWrite {}", action, file.ServerPath, file.LocalPath, file.Id, lastWriteUtc.Ticks);
            var watcher = new Watcher
            {
                LocalPath = file.LocalPath,
                ServerPath = file.ServerPath,
                DeviceIdentifier = Device.DefaultDevice.Value,
            };
            var directory = Path.GetDirectoryName(file.LocalPath)!;
            var name = Path.GetFileName(file.LocalPath);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed on process file");
            return false;
        }
    }
}
