using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class personelMedya : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_PersonelMedyalari_MedyaId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PersonelMedyalari");

            migrationBuilder.RenameColumn(
                name: "MedyaId",
                table: "Personeller",
                newName: "PersonelMedyaId");

            migrationBuilder.RenameIndex(
                name: "IX_Personeller_MedyaId",
                table: "Personeller",
                newName: "IX_Personeller_PersonelMedyaId");

            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "PersonelMedyalari",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMedyalari_PersonelId",
                table: "PersonelMedyalari",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_PersonelMedyalari_PersonelMedyaId",
                table: "Personeller",
                column: "PersonelMedyaId",
                principalTable: "PersonelMedyalari",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonelMedyalari_Personeller_PersonelId",
                table: "PersonelMedyalari",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_PersonelMedyalari_PersonelMedyaId",
                table: "Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonelMedyalari_Personeller_PersonelId",
                table: "PersonelMedyalari");

            migrationBuilder.DropIndex(
                name: "IX_PersonelMedyalari_PersonelId",
                table: "PersonelMedyalari");

            migrationBuilder.DropColumn(
                name: "PersonelId",
                table: "PersonelMedyalari");

            migrationBuilder.RenameColumn(
                name: "PersonelMedyaId",
                table: "Personeller",
                newName: "MedyaId");

            migrationBuilder.RenameIndex(
                name: "IX_Personeller_PersonelMedyaId",
                table: "Personeller",
                newName: "IX_Personeller_MedyaId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PersonelMedyalari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_PersonelMedyalari_MedyaId",
                table: "Personeller",
                column: "MedyaId",
                principalTable: "PersonelMedyalari",
                principalColumn: "Id");
        }
    }
}
