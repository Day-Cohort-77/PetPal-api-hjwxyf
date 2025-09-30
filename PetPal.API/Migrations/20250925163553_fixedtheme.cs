using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetPal.API.Migrations
{
    /// <inheritdoc />
    public partial class fixedtheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ThemeSettings",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "system",
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ThemeSettings",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "system");
        }
    }
}
