using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantumBasedQuantTrading.Repository.Migrations
{
    public partial class addRequestParmModelfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestParametersSet",
                table: "RequestParametersSet");

            migrationBuilder.RenameTable(
                name: "RequestParametersSet",
                newName: "fu_RequestParameters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_fu_RequestParameters",
                table: "fu_RequestParameters",
                column: "RequestParamID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_fu_RequestParameters",
                table: "fu_RequestParameters");

            migrationBuilder.RenameTable(
                name: "fu_RequestParameters",
                newName: "RequestParametersSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestParametersSet",
                table: "RequestParametersSet",
                column: "RequestParamID");
        }
    }
}
