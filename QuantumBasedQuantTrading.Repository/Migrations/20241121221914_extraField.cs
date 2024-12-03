using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantumBasedQuantTrading.Repository.Migrations
{
    public partial class extraField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "fu_MLParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "fu_MLParameters");
        }
    }
}
