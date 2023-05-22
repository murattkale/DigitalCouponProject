using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Migrations
{
    public partial class db4545 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Android_Current",
                table: "SiteConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Android_Min",
                table: "SiteConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ios_Current",
                table: "SiteConfig",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ios_Min",
                table: "SiteConfig",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Android_Current",
                table: "SiteConfig");

            migrationBuilder.DropColumn(
                name: "Android_Min",
                table: "SiteConfig");

            migrationBuilder.DropColumn(
                name: "Ios_Current",
                table: "SiteConfig");

            migrationBuilder.DropColumn(
                name: "Ios_Min",
                table: "SiteConfig");
        }
    }
}
