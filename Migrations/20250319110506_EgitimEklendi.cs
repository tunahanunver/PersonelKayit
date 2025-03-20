using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class EgitimEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonelEgitimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OkulAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bolum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MezuniyetTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DevamEdiyor = table.Column<bool>(type: "bit", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelEgitimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelEgitimleri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonelEgitimleri_PersonelId",
                table: "PersonelEgitimleri",
                column: "PersonelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonelEgitimleri");
        }
    }
}
