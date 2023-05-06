using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class AttendanceHistoryTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceHistory",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NormalizedDegree = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NormalizedBranch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceHistory", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_AttendanceHistory_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceHistory_Degrees_NormalizedDegree",
                        column: x => x.NormalizedDegree,
                        principalTable: "Degrees",
                        principalColumn: "NormalizedDegree",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceHistory_CourseId",
                table: "AttendanceHistory",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceHistory_NormalizedDegree",
                table: "AttendanceHistory",
                column: "NormalizedDegree");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceHistory");
        }
    }
}
