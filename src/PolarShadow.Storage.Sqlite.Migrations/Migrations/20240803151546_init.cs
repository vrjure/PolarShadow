using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolarShadow.Storage.Sqlite.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResourceName = table.Column<string>(type: "TEXT", nullable: true),
                    Progress = table.Column<long>(type: "INTEGER", nullable: false),
                    ProgressDesc = table.Column<string>(type: "TEXT", nullable: true),
                    ProgressIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false),
                    RootId = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Src = table.Column<string>(type: "TEXT", nullable: true),
                    SrcType = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Site = table.Column<string>(type: "TEXT", nullable: true),
                    Request = table.Column<string>(type: "TEXT", nullable: true),
                    FromRequest = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ImageSrc = table.Column<string>(type: "TEXT", nullable: true),
                    ImageSrcHeaders = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Histories_ResourceName",
                table: "Histories",
                column: "ResourceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
