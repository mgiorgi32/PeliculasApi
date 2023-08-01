using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasApi.Migrations
{
    public partial class SalaDeCineData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitud",
                table: "SalaDeCines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitud",
                table: "SalaDeCines",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "SalaDeCines");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "SalaDeCines");
        }
    }
}
