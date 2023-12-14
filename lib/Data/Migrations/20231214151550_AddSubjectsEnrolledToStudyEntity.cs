using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectsEnrolledToStudyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectsEnrolled",
                table: "Studies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectsEnrolledThreshold",
                table: "Studies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectsEnrolled",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "SubjectsEnrolledThreshold",
                table: "Studies");
        }
    }
}
