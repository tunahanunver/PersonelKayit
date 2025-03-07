using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class medyakutuphanesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_PersonelMedyalari_PersonelMedyaId",
                table: "Personeller");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_PersonelMedyaId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "PersonelMedyaId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MedyaKutuphaneleri");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "MedyaKutuphaneleri",
                newName: "MedyaAdi");

            migrationBuilder.AddColumn<int>(
                name: "PersonelMedyaId",
                table: "MedyaKutuphaneleri",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedyaKutuphaneleri_PersonelMedyaId",
                table: "MedyaKutuphaneleri",
                column: "PersonelMedyaId",
                unique: true,
                filter: "[PersonelMedyaId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MedyaKutuphaneleri_PersonelMedyalari_PersonelMedyaId",
                table: "MedyaKutuphaneleri",
                column: "PersonelMedyaId",
                principalTable: "PersonelMedyalari",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedyaKutuphaneleri_PersonelMedyalari_PersonelMedyaId",
                table: "MedyaKutuphaneleri");

            migrationBuilder.DropIndex(
                name: "IX_MedyaKutuphaneleri_PersonelMedyaId",
                table: "MedyaKutuphaneleri");

            migrationBuilder.DropColumn(
                name: "PersonelMedyaId",
                table: "MedyaKutuphaneleri");

            migrationBuilder.RenameColumn(
                name: "MedyaAdi",
                table: "MedyaKutuphaneleri",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "PersonelMedyaId",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MedyaKutuphaneleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_PersonelMedyaId",
                table: "Personeller",
                column: "PersonelMedyaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_PersonelMedyalari_PersonelMedyaId",
                table: "Personeller",
                column: "PersonelMedyaId",
                principalTable: "PersonelMedyalari",
                principalColumn: "Id");
        }
    }
}
