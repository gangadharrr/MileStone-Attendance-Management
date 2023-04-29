using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class BranchesFKmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Degrees_DegreesNormalizedDegree",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_DegreesNormalizedDegree",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "DegreesNormalizedDegree",
                table: "Branches");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedDegree",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_NormalizedDegree",
                table: "Branches",
                column: "NormalizedDegree");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Degrees_NormalizedDegree",
                table: "Branches",
                column: "NormalizedDegree",
                principalTable: "Degrees",
                principalColumn: "NormalizedDegree",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Degrees_NormalizedDegree",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_NormalizedDegree",
                table: "Branches");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedDegree",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "DegreesNormalizedDegree",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_DegreesNormalizedDegree",
                table: "Branches",
                column: "DegreesNormalizedDegree");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Degrees_DegreesNormalizedDegree",
                table: "Branches",
                column: "DegreesNormalizedDegree",
                principalTable: "Degrees",
                principalColumn: "NormalizedDegree",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
