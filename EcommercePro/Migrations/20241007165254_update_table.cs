using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class update_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Guarantees");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Guarantees",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Guarantees");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Guarantees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
