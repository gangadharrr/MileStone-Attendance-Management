using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class BatchAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Batch",
                table: "AttendanceHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Batch",
                table: "AttendanceHistory");
        }
    }
}
