using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmeDhcpServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetworkConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Gateway1 = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    Gateway2 = table.Column<string>(type: "nvarchar(45)", nullable: true),
                    Dhcp = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    SubnetMask = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    Dns1 = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    Dns2 = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetworkDevices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PresentedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddress = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    Permanent = table.Column<bool>(type: "bit", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaseTimeSeconds = table.Column<int>(type: "int", nullable: false),
                    PreferredNetworkConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkDevices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetworkConfigurations");

            migrationBuilder.DropTable(
                name: "NetworkDevices");
        }
    }
}
