using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetPal.API.Migrations
{
    /// <inheritdoc />
    public partial class updateduserprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThemeSettings",
                table: "UserProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ColorAccent",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "#4a90e2");

            migrationBuilder.AddColumn<string>(
                name: "FontSize",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "medium");

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "light");

            migrationBuilder.AddColumn<bool>(
                name: "UseSystemPreference",
                table: "UserProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorAccent",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "UseSystemPreference",
                table: "UserProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ThemeSettings",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "system");
        }
    }
}
