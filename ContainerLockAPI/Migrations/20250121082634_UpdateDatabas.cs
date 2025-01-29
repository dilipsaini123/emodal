using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContainerLockAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ContainerLock",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "L",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "A");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ContainerLock",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "A",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "L");
        }
    }
}
