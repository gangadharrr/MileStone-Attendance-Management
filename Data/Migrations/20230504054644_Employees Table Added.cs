using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MileStone_Attendance_Management.Data.Migrations
{
    public partial class EmployeesTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NormalizedDegree = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NormalizedBranch = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Email);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetRoles_Designation",
                        column: x => x.Designation,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Degrees_NormalizedDegree",
                        column: x => x.NormalizedDegree,
                        principalTable: "Degrees",
                        principalColumn: "NormalizedDegree",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Designation",
                table: "Employees",
                column: "Designation");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeId",
                table: "Employees",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NormalizedDegree",
                table: "Employees",
                column: "NormalizedDegree");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
