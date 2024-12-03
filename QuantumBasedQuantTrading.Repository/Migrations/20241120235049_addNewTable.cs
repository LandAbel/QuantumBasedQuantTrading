using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantumBasedQuantTrading.Repository.Migrations
{
    public partial class addNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fu_MLParameters",
                columns: table => new
                {
                    MLModelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleSentiment = table.Column<float>(type: "real", nullable: false),
                    ContSentiment = table.Column<float>(type: "real", nullable: false),
                    DescSentiment = table.Column<float>(type: "real", nullable: false),
                    OpenPrice = table.Column<float>(type: "real", nullable: false),
                    CurrentHighPrice = table.Column<float>(type: "real", nullable: false),
                    CurrentLowPrice = table.Column<float>(type: "real", nullable: false),
                    CurrentVolume = table.Column<float>(type: "real", nullable: false),
                    trainMAE = table.Column<double>(type: "float", nullable: false),
                    valMAE = table.Column<double>(type: "float", nullable: false),
                    testMAE = table.Column<double>(type: "float", nullable: false),
                    predictedValue = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fu_MLParameters", x => x.MLModelID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fu_MLParameters");
        }
    }
}
