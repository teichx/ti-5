using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TI5yncronizer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pending_synchronizer_device_identifier_local_path_old_local_path_action_last_write_utc_as_ticks",
                table: "pending_synchronizer");

            migrationBuilder.RenameColumn(
                name: "old_local_path",
                table: "pending_synchronizer",
                newName: "old_server_path");

            migrationBuilder.AddColumn<string>(
                name: "server_path",
                table: "pending_synchronizer",
                type: "TEXT",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "server_path",
                table: "pending_synchronizer");

            migrationBuilder.RenameColumn(
                name: "old_server_path",
                table: "pending_synchronizer",
                newName: "old_local_path");

            migrationBuilder.CreateIndex(
                name: "ix_pending_synchronizer_device_identifier_local_path_old_local_path_action_last_write_utc_as_ticks",
                table: "pending_synchronizer",
                columns: new[] { "device_identifier", "local_path", "old_local_path", "action", "last_write_utc_as_ticks" });
        }
    }
}
