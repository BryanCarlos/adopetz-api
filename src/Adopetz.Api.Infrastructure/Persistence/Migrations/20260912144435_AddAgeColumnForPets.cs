using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adopetz.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeColumnForPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "age_in_years",
                table: "pets",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age_in_years",
                table: "pets");
        }
    }
}
