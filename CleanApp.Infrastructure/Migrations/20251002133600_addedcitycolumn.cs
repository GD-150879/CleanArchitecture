using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedcitycolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Employees");
        }
    }
}
