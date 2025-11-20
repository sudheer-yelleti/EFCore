using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_CourseEnrollments_CourseEnrollmentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_CourseEnrollmentId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CourseEnrollmentId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "CourseEnrollments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentId",
                table: "CourseEnrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Students_StudentId",
                table: "CourseEnrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Students_StudentId",
                table: "CourseEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrollments_StudentId",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "CourseEnrollments");

            migrationBuilder.AddColumn<int>(
                name: "CourseEnrollmentId",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_CourseEnrollmentId",
                table: "Students",
                column: "CourseEnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_CourseEnrollments_CourseEnrollmentId",
                table: "Students",
                column: "CourseEnrollmentId",
                principalTable: "CourseEnrollments",
                principalColumn: "CourseEnrollmentId");
        }
    }
}
