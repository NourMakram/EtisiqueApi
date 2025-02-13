using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class addnewcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeElapsedClose1",
                table: "RequestCommonParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeElapsedClose2",
                table: "RequestCommonParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ApartmentServicesRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeElapsedClose1",
                table: "RequestCommonParts");

            migrationBuilder.DropColumn(
                name: "TimeElapsedClose2",
                table: "RequestCommonParts");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ApartmentServicesRequests");
        }
    }
}
