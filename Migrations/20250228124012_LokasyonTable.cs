using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class LokasyonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LokasyonId",
                table: "Personeller",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Lokasyonlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lokasyonlar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_LokasyonId",
                table: "Personeller",
                column: "LokasyonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_Lokasyonlar_LokasyonId",
                table: "Personeller",
                column: "LokasyonId",
                principalTable: "Lokasyonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_Lokasyonlar_LokasyonId",
                table: "Personeller");

            migrationBuilder.DropTable(
                name: "Lokasyonlar");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_LokasyonId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "LokasyonId",
                table: "Personeller");
        }
    }
}
