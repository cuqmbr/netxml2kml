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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mac = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    TotalPackets = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirelessClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WirelessNetworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ssid = table.Column<string>(type: "TEXT", nullable: false),
                    IsCloaked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Encryption = table.Column<string>(type: "TEXT", nullable: false),
                    Bssid = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    FrequencyMhz = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxSignalDbm = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLatitude = table.Column<double>(type: "REAL", nullable: false),
                    MaxLongitude = table.Column<double>(type: "REAL", nullable: false),
                    MaxAltitude = table.Column<double>(type: "REAL", nullable: false),
                    FirstSeen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalPackets = table.Column<int>(type: "INTEGER", nullable: false),
                    WirelessClientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirelessNetworks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WirelessNetworks_WirelessClients_WirelessClientId",
                        column: x => x.WirelessClientId,
                        principalTable: "WirelessClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WirelessNetworks_WirelessClientId",
                table: "WirelessNetworks",
                column: "WirelessClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WirelessNetworks");

            migrationBuilder.DropTable(
                name: "WirelessClients");
        }
    }
}
