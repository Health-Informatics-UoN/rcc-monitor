using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReportStatusNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportStatus_StatusId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportStatus_StatusId",
                table: "Reports",
                column: "StatusId",
                principalTable: "ReportStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportStatus_StatusId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportStatus_StatusId",
                table: "Reports",
                column: "StatusId",
                principalTable: "ReportStatus",
                principalColumn: "Id");
        }
    }
}
