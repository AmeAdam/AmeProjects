using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmeServer.Migrations
{
    public partial class ConfigPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreferredNetworkConfiguration",
                table: "NetworkDevices",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "NetworkConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "NetworkConfigurations");

            migrationBuilder.AlterColumn<string>(
                name: "PreferredNetworkConfiguration",
                table: "NetworkDevices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
