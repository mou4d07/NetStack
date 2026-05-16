using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetMapManager.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cabinets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlueprintImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cabinets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatchCordType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetworkSwitches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPorts = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentSwitchId = table.Column<int>(type: "int", nullable: true),
                    CabinetId = table.Column<int>(type: "int", nullable: false),
                    XCoordinate = table.Column<double>(type: "float", nullable: false),
                    YCoordinate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkSwitches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkSwitches_Cabinets_CabinetId",
                        column: x => x.CabinetId,
                        principalTable: "Cabinets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NetworkSwitches_NetworkSwitches_ParentSwitchId",
                        column: x => x.ParentSwitchId,
                        principalTable: "NetworkSwitches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceSwitchId = table.Column<int>(type: "int", nullable: false),
                    SourcePort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationSwitchId = table.Column<int>(type: "int", nullable: false),
                    DestinationPort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatchCordType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Links_NetworkSwitches_DestinationSwitchId",
                        column: x => x.DestinationSwitchId,
                        principalTable: "NetworkSwitches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Links_NetworkSwitches_SourceSwitchId",
                        column: x => x.SourceSwitchId,
                        principalTable: "NetworkSwitches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SwitchId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceLogs_NetworkSwitches_SwitchId",
                        column: x => x.SwitchId,
                        principalTable: "NetworkSwitches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedUsers_DomainUsername",
                table: "AuthorizedUsers",
                column: "DomainUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Links_DestinationSwitchId",
                table: "Links",
                column: "DestinationSwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_Links_SourceSwitchId",
                table: "Links",
                column: "SourceSwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLogs_SwitchId",
                table: "MaintenanceLogs",
                column: "SwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkSwitches_CabinetId",
                table: "NetworkSwitches",
                column: "CabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkSwitches_ParentSwitchId",
                table: "NetworkSwitches",
                column: "ParentSwitchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedUsers");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "MaintenanceLogs");

            migrationBuilder.DropTable(
                name: "NetworkSwitches");

            migrationBuilder.DropTable(
                name: "Cabinets");
        }
    }
}
