using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class CoursesAssignedTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoursesAssigned",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    NormalizedDegree = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NormalizedBranch = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesAssigned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoursesAssigned_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesAssigned_Degrees_NormalizedDegree",
                        column: x => x.NormalizedDegree,
                        principalTable: "Degrees",
                        principalColumn: "NormalizedDegree",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursesAssigned_CourseId",
                table: "CoursesAssigned",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesAssigned_NormalizedDegree",
                table: "CoursesAssigned",
                column: "NormalizedDegree");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesAssigned");
        }
    }
}
