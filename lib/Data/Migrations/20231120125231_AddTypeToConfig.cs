﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Config",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Config");
        }
    }
}
