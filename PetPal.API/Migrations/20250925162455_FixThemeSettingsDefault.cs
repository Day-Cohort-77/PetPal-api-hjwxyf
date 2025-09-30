using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetPal.API.Migrations
{
    /// <inheritdoc />
    public partial class FixThemeSettingsDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill existing rows (empty or null) to 'system'
            migrationBuilder.Sql("""
            UPDATE "UserProfiles"
            SET "ThemeSettings" = 'system'
            WHERE "ThemeSettings" IS NULL OR "ThemeSettings" = '';
            """);
            migrationBuilder.Sql("""
            ALTER TABLE "UserProfiles"
            ALTER COLUMN "ThemeSettings" SET DEFAULT 'system';
            """);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql("""
            ALTER TABLE "UserProfiles"
            ALTER COLUMN "ThemeSettings" SET DEFAULT '';
            """);

        }
    }
}
