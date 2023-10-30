using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyUser_Studies_StudyId",
                table: "StudyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyUser",
                table: "StudyUser");

            migrationBuilder.RenameTable(
                name: "StudyUser",
                newName: "StudyUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyUsers",
                table: "StudyUsers",
                columns: new[] { "StudyId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudyUsers_Studies_StudyId",
                table: "StudyUsers",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "RedCapId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyUsers_Studies_StudyId",
                table: "StudyUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyUsers",
                table: "StudyUsers");

            migrationBuilder.RenameTable(
                name: "StudyUsers",
                newName: "StudyUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyUser",
                table: "StudyUser",
                columns: new[] { "StudyId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudyUser_Studies_StudyId",
                table: "StudyUser",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "RedCapId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
