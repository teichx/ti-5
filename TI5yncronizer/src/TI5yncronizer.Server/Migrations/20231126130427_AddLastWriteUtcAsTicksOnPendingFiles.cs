using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TI5yncronizer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddLastWriteUtcAsTicksOnPendingFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "last_write_utc_as_ticks",
                table: "pending_synchronizer",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_pending_synchronizer_device_identifier_local_path_old_local_path_action_last_write_utc_as_ticks",
                table: "pending_synchronizer",
                columns: new[] { "device_identifier", "local_path", "old_local_path", "action", "last_write_utc_as_ticks" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pending_synchronizer_device_identifier_local_path_old_local_path_action_last_write_utc_as_ticks",
                table: "pending_synchronizer");

            migrationBuilder.DropColumn(
                name: "last_write_utc_as_ticks",
                table: "pending_synchronizer");
        }
    }
}
