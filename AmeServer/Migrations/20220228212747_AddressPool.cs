using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmeServer.Migrations
{
    public partial class AddressPool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PoolMax",
                table: "NetworkConfigurations",
                type: "nvarchar(45)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PoolMin",
                table: "NetworkConfigurations",
                type: "nvarchar(45)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PoolMax",
                table: "NetworkConfigurations");

            migrationBuilder.DropColumn(
                name: "PoolMin",
                table: "NetworkConfigurations");
        }
    }
}
