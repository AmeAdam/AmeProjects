using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmeServer.Migrations
{
    public partial class NetworkConfiguration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dns1",
                table: "NetworkConfigurations");

            migrationBuilder.DropColumn(
                name: "Dns2",
                table: "NetworkConfigurations");

            migrationBuilder.DropColumn(
                name: "Gateway1",
                table: "NetworkConfigurations");

            migrationBuilder.DropColumn(
                name: "Gateway2",
                table: "NetworkConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "Dns",
                table: "NetworkConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gateways",
                table: "NetworkConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dns",
                table: "NetworkConfigurations");

            migrationBuilder.DropColumn(
                name: "Gateways",
                table: "NetworkConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "Dns1",
                table: "NetworkConfigurations",
                type: "nvarchar(45)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Dns2",
                table: "NetworkConfigurations",
                type: "nvarchar(45)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gateway1",
                table: "NetworkConfigurations",
                type: "nvarchar(45)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gateway2",
                table: "NetworkConfigurations",
                type: "nvarchar(45)",
                nullable: true);
        }
    }
}
