using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TI5yncronizer.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "listener",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    local_path = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    server_path = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    device_identifier = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listener", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pending_synchronizer",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    local_path = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    old_local_path = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    action = table.Column<int>(type: "INTEGER", nullable: false),
                    device_identifier = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pending_synchronizer", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listener");

            migrationBuilder.DropTable(
                name: "pending_synchronizer");
        }
    }
}
