using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
}
