using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetMapManager.API.Migrations
{
    /// <inheritdoc />
    public partial class RemovePatchCord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatchCordType",
                table: "Links");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatchCordType",
                table: "Links",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
