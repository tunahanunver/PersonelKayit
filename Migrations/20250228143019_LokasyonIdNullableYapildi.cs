using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class LokasyonIdNullableYapildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_Lokasyonlar_LokasyonId",
                table: "Personeller");

            migrationBuilder.AlterColumn<int>(
                name: "LokasyonId",
                table: "Personeller",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_Lokasyonlar_LokasyonId",
                table: "Personeller",
                column: "LokasyonId",
                principalTable: "Lokasyonlar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_Lokasyonlar_LokasyonId",
                table: "Personeller");

            migrationBuilder.AlterColumn<int>(
                name: "LokasyonId",
                table: "Personeller",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_Lokasyonlar_LokasyonId",
                table: "Personeller",
                column: "LokasyonId",
                principalTable: "Lokasyonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
