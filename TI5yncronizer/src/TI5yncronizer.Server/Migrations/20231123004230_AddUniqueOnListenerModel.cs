using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TI5yncronizer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueOnListenerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_listener_device_identifier_local_path_server_path",
                table: "listener",
                columns: new[] { "device_identifier", "local_path", "server_path" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_listener_device_identifier_local_path_server_path",
                table: "listener");
        }
    }
}
