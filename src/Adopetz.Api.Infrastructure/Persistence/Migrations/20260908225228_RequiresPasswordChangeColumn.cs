using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adopetz.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RequiresPasswordChangeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "requires_password_change",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "requires_password_change",
                table: "users");
        }
    }
}
