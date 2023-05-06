using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class AttendanceTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false),
                    RollNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentStatus = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Attendance_AttendanceHistory_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "AttendanceHistory",
                        principalColumn: "AttendanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Students_Email",
                        column: x => x.Email,
                        principalTable: "Students",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.NoAction);

                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_AttendanceId",
                table: "Attendance",
                column: "AttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Email",
                table: "Attendance",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_RollNumber",
                table: "Attendance",
                column: "RollNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");
        }
    }
}
