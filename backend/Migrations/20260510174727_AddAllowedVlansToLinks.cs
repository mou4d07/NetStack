using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetMapManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowedVlansToLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedVlans",
                table: "Links",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedVlans",
                table: "Links");
        }
    }
}
