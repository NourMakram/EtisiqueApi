using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class editcustomertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "GuaranteeEnd",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "GuaranteeStart",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "projectId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_projectId",
                table: "Customers",
                column: "projectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Projects_projectId",
                table: "Customers",
                column: "projectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Projects_projectId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_projectId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "GuaranteeEnd",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "GuaranteeStart",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "projectId",
                table: "Customers");
        }
    }
}
