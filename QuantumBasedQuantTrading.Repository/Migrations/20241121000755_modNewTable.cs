using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantumBasedQuantTrading.Repository.Migrations
{
    public partial class modNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContSentiment",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "CurrentHighPrice",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "CurrentLowPrice",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "CurrentVolume",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "DescSentiment",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "OpenPrice",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "fu_MLParameters");

            migrationBuilder.DropColumn(
                name: "TitleSentiment",
                table: "fu_MLParameters");

            migrationBuilder.RenameColumn(
                name: "MLModelID",
                table: "fu_MLParameters",
                newName: "MLResultID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MLResultID",
                table: "fu_MLParameters",
                newName: "MLModelID");

            migrationBuilder.AddColumn<float>(
                name: "ContSentiment",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CurrentHighPrice",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CurrentLowPrice",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CurrentVolume",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DescSentiment",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "OpenPrice",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "fu_MLParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "TitleSentiment",
                table: "fu_MLParameters",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
