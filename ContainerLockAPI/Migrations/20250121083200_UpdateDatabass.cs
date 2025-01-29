using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContainerLockAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContainerLock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ContainerLock",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "L");
        }
    }
}
