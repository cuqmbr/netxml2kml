using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace netxml2kml.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WirelessClients",
                columns: table => new
                {
                    Mac = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSeenDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirelessClients", x => x.Mac);
                });

            migrationBuilder.CreateTable(
                name: "WirelessNetworks",
                columns: table => new
                {
                    Bssid = table.Column<string>(type: "TEXT", nullable: false),
                    Essid = table.Column<string>(type: "TEXT", nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    Encryption = table.Column<string>(type: "TEXT", nullable: true),
                    FrequencyMhz = table.Column<double>(type: "REAL", nullable: false),
                    MaxSignalDbm = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLatitude = table.Column<double>(type: "REAL", nullable: false),
                    MaxLongitude = table.Column<double>(type: "REAL", nullable: false),
                    MaxAltitude = table.Column<double>(type: "REAL", nullable: false),
                    FirstSeenDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirelessNetworks", x => x.Bssid);
                });

            migrationBuilder.CreateTable(
                name: "WirelessConnections",
                columns: table => new
                {
                    WirelessNetworkBssid = table.Column<string>(type: "TEXT", nullable: false),
                    WirelessClientMac = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSeenDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirelessConnections", x => new { x.WirelessNetworkBssid, x.WirelessClientMac });
                    table.ForeignKey(
                        name: "FK_WirelessConnections_WirelessClients_WirelessClientMac",
                        column: x => x.WirelessClientMac,
                        principalTable: "WirelessClients",
                        principalColumn: "Mac",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WirelessConnections_WirelessNetworks_WirelessNetworkBssid",
                        column: x => x.WirelessNetworkBssid,
                        principalTable: "WirelessNetworks",
                        principalColumn: "Bssid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WirelessConnections_WirelessClientMac",
                table: "WirelessConnections",
                column: "WirelessClientMac");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WirelessConnections");

            migrationBuilder.DropTable(
                name: "WirelessClients");

            migrationBuilder.DropTable(
                name: "WirelessNetworks");
        }
    }
}
