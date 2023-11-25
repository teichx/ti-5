using System.ComponentModel.DataAnnotations;
using TI5yncronizer.Core;

namespace TI5yncronizer.Server.Model;

public class PendingSynchronizeModel
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(512)]
    public required string LocalPath { get; set; }

    [StringLength(512)]
    public string? OldLocalPath { get; set; }

    [Required]
    public required EnumAction Action { get; set; }

    [Required, StringLength(255)]
    public required string DeviceIdentifier { get; set; }

    [Required]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
