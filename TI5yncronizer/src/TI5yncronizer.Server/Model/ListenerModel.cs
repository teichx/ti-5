using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TI5yncronizer.Core.FileWatcher;

namespace TI5yncronizer.Server.Model;

[Index("DeviceIdentifier", "LocalPath", "ServerPath", IsUnique = true)]
public class ListenerModel
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(512)]
    public required string LocalPath { get; set; }

    [Required, StringLength(512)]
    public required string ServerPath { get; set; }

    [Required, StringLength(255)]
    public required string DeviceIdentifier { get; set; }

    [Required]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } = null;

    public static ListenerModel FromWatcher(Watcher watcher)
        => new()
        {
            LocalPath = watcher.LocalPath,
            ServerPath = watcher.ServerPath,
            DeviceIdentifier = watcher.DeviceIdentifier,
        };

    public Watcher ToWatcher()
        => new()
        {
            LocalPath = LocalPath,
            ServerPath = ServerPath,
            DeviceIdentifier = DeviceIdentifier,
        };
}
