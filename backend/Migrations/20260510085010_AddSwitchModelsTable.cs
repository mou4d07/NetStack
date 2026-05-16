using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetMapManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSwitchModelsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "NetworkSwitches");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "NetworkSwitches");

            migrationBuilder.AddColumn<int>(
                name: "SwitchModelId",
                table: "NetworkSwitches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SwitchModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwitchModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkSwitches_SwitchModelId",
                table: "NetworkSwitches",
                column: "SwitchModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_NetworkSwitches_SwitchModels_SwitchModelId",
                table: "NetworkSwitches",
                column: "SwitchModelId",
                principalTable: "SwitchModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetworkSwitches_SwitchModels_SwitchModelId",
                table: "NetworkSwitches");

            migrationBuilder.DropTable(
                name: "SwitchModels");

            migrationBuilder.DropIndex(
                name: "IX_NetworkSwitches_SwitchModelId",
                table: "NetworkSwitches");

            migrationBuilder.DropColumn(
                name: "SwitchModelId",
                table: "NetworkSwitches");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "NetworkSwitches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "NetworkSwitches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
