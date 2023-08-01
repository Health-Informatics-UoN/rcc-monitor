using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Monitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReportInstanceNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstanceId",
                table: "Reports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Instances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_InstanceId",
                table: "Reports",
                column: "InstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Instances_InstanceId",
                table: "Reports",
                column: "InstanceId",
                principalTable: "Instances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Instances_InstanceId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Reports_InstanceId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "InstanceId",
                table: "Reports");
        }
    }
}
