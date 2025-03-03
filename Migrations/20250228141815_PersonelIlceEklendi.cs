using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonelKayit.Migrations
{
    /// <inheritdoc />
    public partial class PersonelIlceEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "Personeller");
        }
    }
}
