using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevOps.Backend.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "DevOps");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DevOps");

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "DevOps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedTo = table.Column<string>(maxLength: 32, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: false),
                    Due = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });
        }
    }
}