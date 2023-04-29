using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class BranchesTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedBranch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedDegree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DegreesNormalizedDegree = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Branches_Degrees_DegreesNormalizedDegree",
                        column: x => x.DegreesNormalizedDegree,
                        principalTable: "Degrees",
                        principalColumn: "NormalizedDegree",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_DegreesNormalizedDegree",
                table: "Branches",
                column: "DegreesNormalizedDegree");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
