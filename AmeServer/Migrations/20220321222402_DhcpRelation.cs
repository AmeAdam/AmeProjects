using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmeServer.Migrations
{
    public partial class DhcpRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredConfigurationId",
                table: "NetworkDevices",
                type: "nvarchar(256)",
                nullable: true);

            migrationBuilder.Sql("UPDATE NetworkDevices SET PreferredConfigurationId=PreferredNetworkConfiguration");

            migrationBuilder.DropColumn(
                name: "PreferredNetworkConfiguration",
                table: "NetworkDevices");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_PreferredConfigurationId",
                table: "NetworkDevices",
                column: "PreferredConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkDevices_NetworkConfigurations_PreferredConfigurationId",
                table: "NetworkDevices",
                column: "PreferredConfigurationId",
                principalTable: "NetworkConfigurations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetworkDevices_NetworkConfigurations_PreferredConfigurationId",
                table: "NetworkDevices");

            migrationBuilder.DropIndex(
                name: "IX_NetworkDevices_PreferredConfigurationId",
                table: "NetworkDevices");

            migrationBuilder.DropColumn(
                name: "PreferredConfigurationId",
                table: "NetworkDevices");

            migrationBuilder.AddColumn<string>(
                name: "PreferredNetworkConfiguration",
                table: "NetworkDevices",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
