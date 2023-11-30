using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstanceId",
                table: "Studies",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "StudyCapacityAlert",
                table: "Studies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StudyCapacityAlertsActivated",
                table: "Studies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StudyCapacityJobFrequency",
                table: "Studies",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(1, 0, 0, 0, 0));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StudyCapacityLastChecked",
                table: "Studies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<double>(
                name: "StudyCapacityThreshold",
                table: "Studies",
                type: "double precision",
                nullable: false,
                defaultValue: 0.75);

            migrationBuilder.CreateTable(
                name: "StudyGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    StudyRedCapId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PlannedSize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyGroups_Studies_StudyRedCapId",
                        column: x => x.StudyRedCapId,
                        principalTable: "Studies",
                        principalColumn: "RedCapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Studies_InstanceId",
                table: "Studies",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroups_StudyRedCapId",
                table: "StudyGroups",
                column: "StudyRedCapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Studies_Instances_InstanceId",
                table: "Studies",
                column: "InstanceId",
                principalTable: "Instances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Studies_Instances_InstanceId",
                table: "Studies");

            migrationBuilder.DropTable(
                name: "StudyGroups");

            migrationBuilder.DropIndex(
                name: "IX_Studies_InstanceId",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "StudyCapacityAlert",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "StudyCapacityAlertsActivated",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "StudyCapacityJobFrequency",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "StudyCapacityLastChecked",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "StudyCapacityThreshold",
                table: "Studies");
        }
    }
}
