using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class EmployeeIdNameToEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionsAssigned_Employees_EmployeeId",
                table: "SectionsAssigned");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "SectionsAssigned",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_SectionsAssigned_EmployeeId",
                table: "SectionsAssigned",
                newName: "IX_SectionsAssigned_Email");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionsAssigned_Employees_Email",
                table: "SectionsAssigned",
                column: "Email",
                principalTable: "Employees",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionsAssigned_Employees_Email",
                table: "SectionsAssigned");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "SectionsAssigned",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_SectionsAssigned_Email",
                table: "SectionsAssigned",
                newName: "IX_SectionsAssigned_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionsAssigned_Employees_EmployeeId",
                table: "SectionsAssigned",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
