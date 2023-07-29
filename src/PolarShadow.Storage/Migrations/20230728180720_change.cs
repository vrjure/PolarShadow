using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolarShadow.Storage.Migrations
{
    /// <inheritdoc />
    public partial class change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiteName",
                table: "MyCollection",
                newName: "Src");

            migrationBuilder.RenameColumn(
                name: "DetailSrc",
                table: "MyCollection",
                newName: "Site");

            migrationBuilder.AddColumn<int>(
                name: "SrcType",
                table: "MyCollection",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SrcType",
                table: "MyCollection");

            migrationBuilder.RenameColumn(
                name: "Src",
                table: "MyCollection",
                newName: "SiteName");

            migrationBuilder.RenameColumn(
                name: "Site",
                table: "MyCollection",
                newName: "DetailSrc");
        }
    }
}
