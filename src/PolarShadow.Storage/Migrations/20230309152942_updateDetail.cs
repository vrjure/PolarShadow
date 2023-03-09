using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolarShadow.Storage.Migrations
{
    /// <inheritdoc />
    public partial class updateDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Seasons",
                table: "MyCollection",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seasons",
                table: "MyCollection");
        }
    }
}
