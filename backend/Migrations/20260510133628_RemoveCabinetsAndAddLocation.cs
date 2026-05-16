using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetMapManager.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCabinetsAndAddLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetworkSwitches_Cabinets_CabinetId",
                table: "NetworkSwitches");

            migrationBuilder.DropTable(
                name: "Cabinets");

            migrationBuilder.DropIndex(
                name: "IX_NetworkSwitches_CabinetId",
                table: "NetworkSwitches");

            migrationBuilder.DropColumn(
                name: "CabinetId",
                table: "NetworkSwitches");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "NetworkSwitches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "NetworkSwitches");

            migrationBuilder.AddColumn<int>(
                name: "CabinetId",
                table: "NetworkSwitches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cabinets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlueprintImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cabinets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkSwitches_CabinetId",
                table: "NetworkSwitches",
                column: "CabinetId");

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkSwitches_Cabinets_CabinetId",
                table: "NetworkSwitches",
                column: "CabinetId",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
