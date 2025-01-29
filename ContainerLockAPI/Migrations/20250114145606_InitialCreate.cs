using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContainerLockAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContainerLock",
                columns: table => new
                {
                    ContainerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LockTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false, computedColumnSql: "DATEADD(MINUTE, 1, LockTimestamp)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerLock", x => x.ContainerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerLock");
        }
    }
}
