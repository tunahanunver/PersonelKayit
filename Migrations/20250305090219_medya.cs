using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class medya : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MedyaId",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedyaKutuphaneleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedyaGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedyaKutuphaneleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonelMedyalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelMedyalari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_MedyaId",
                table: "Personeller",
                column: "MedyaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_PersonelMedyalari_MedyaId",
                table: "Personeller",
                column: "MedyaId",
                principalTable: "PersonelMedyalari",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_PersonelMedyalari_MedyaId",
                table: "Personeller");

            migrationBuilder.DropTable(
                name: "MedyaKutuphaneleri");

            migrationBuilder.DropTable(
                name: "PersonelMedyalari");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_MedyaId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "MedyaId",
                table: "Personeller");
        }
    }
}
