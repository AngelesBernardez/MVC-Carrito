using Microsoft.EntityFrameworkCore.Migrations;

namespace B_G2_CarritoCompras.Migrations
{
    public partial class RemovidoBooleanoCompradoEnItemCompra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comprado",
                table: "ItemsCompras");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Comprado",
                table: "ItemsCompras",
                nullable: false,
                defaultValue: false);
        }
    }
}
