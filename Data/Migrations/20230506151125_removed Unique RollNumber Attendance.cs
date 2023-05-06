using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class removedUniqueRollNumberAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendance_RollNumber",
                table: "Attendance");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendance_RollNumber",
                table: "Attendance",
                column: "RollNumber",
                unique: true);
        }
    }
}
