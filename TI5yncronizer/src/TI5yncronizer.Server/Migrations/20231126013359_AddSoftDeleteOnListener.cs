using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TI5yncronizer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteOnListener : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "listener",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "listener");
        }
    }
}
