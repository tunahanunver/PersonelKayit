using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class AdresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdresId",
                table: "Lokasyonlar",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Adresler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresler", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lokasyonlar_AdresId",
                table: "Lokasyonlar",
                column: "AdresId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lokasyonlar_Adresler_AdresId",
                table: "Lokasyonlar",
                column: "AdresId",
                principalTable: "Adresler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lokasyonlar_Adresler_AdresId",
                table: "Lokasyonlar");

            migrationBuilder.DropTable(
                name: "Adresler");

            migrationBuilder.DropIndex(
                name: "IX_Lokasyonlar_AdresId",
                table: "Lokasyonlar");

            migrationBuilder.DropColumn(
                name: "AdresId",
                table: "Lokasyonlar");
        }
    }
}
