using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetPal.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedicationPrescribedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prescriber",
                table: "Medications",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "PrescriberId",
                table: "Medications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrescriberName",
                table: "Medications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ReminderEnabled",
                table: "Medications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrescriberId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "PrescriberName",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ReminderEnabled",
                table: "Medications");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Medications",
                newName: "Prescriber");
        }
    }
}
