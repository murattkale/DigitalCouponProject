using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Migrations
{
    public partial class db34dff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMaintenance",
                table: "SiteConfig",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceEndDate",
                table: "SiteConfig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceStartDate",
                table: "SiteConfig",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMaintenance",
                table: "SiteConfig");

            migrationBuilder.DropColumn(
                name: "MaintenanceEndDate",
                table: "SiteConfig");

            migrationBuilder.DropColumn(
                name: "MaintenanceStartDate",
                table: "SiteConfig");
        }
    }
}
