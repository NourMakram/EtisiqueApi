using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class edit23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestImage",
                table: "KitchenServices",
                newName: "TimeElapsed");

            migrationBuilder.AddColumn<int>(
                name: "ApartmentServicesRequestid",
                table: "RequestsImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KitchenServicesid",
                table: "RequestsImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestsCommonPartsid",
                table: "RequestsImages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestImage",
                table: "RequestCommonParts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TimeElapsed",
                table: "RequestCommonParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TimeElapsed",
                table: "Emergencies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestImage",
                table: "ApartmentServicesRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "hasGuarantee",
                table: "ApartmentServicesRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestsImages_ApartmentServicesRequestid",
                table: "RequestsImages",
                column: "ApartmentServicesRequestid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestsImages_KitchenServicesid",
                table: "RequestsImages",
                column: "KitchenServicesid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestsImages_RequestsCommonPartsid",
                table: "RequestsImages",
                column: "RequestsCommonPartsid");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestsImages_ApartmentServicesRequests_ApartmentServicesRequestid",
                table: "RequestsImages",
                column: "ApartmentServicesRequestid",
                principalTable: "ApartmentServicesRequests",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestsImages_KitchenServices_KitchenServicesid",
                table: "RequestsImages",
                column: "KitchenServicesid",
                principalTable: "KitchenServices",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestsImages_RequestCommonParts_RequestsCommonPartsid",
                table: "RequestsImages",
                column: "RequestsCommonPartsid",
                principalTable: "RequestCommonParts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestsImages_ApartmentServicesRequests_ApartmentServicesRequestid",
                table: "RequestsImages");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestsImages_KitchenServices_KitchenServicesid",
                table: "RequestsImages");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestsImages_RequestCommonParts_RequestsCommonPartsid",
                table: "RequestsImages");

            migrationBuilder.DropIndex(
                name: "IX_RequestsImages_ApartmentServicesRequestid",
                table: "RequestsImages");

            migrationBuilder.DropIndex(
                name: "IX_RequestsImages_KitchenServicesid",
                table: "RequestsImages");

            migrationBuilder.DropIndex(
                name: "IX_RequestsImages_RequestsCommonPartsid",
                table: "RequestsImages");

            migrationBuilder.DropColumn(
                name: "ApartmentServicesRequestid",
                table: "RequestsImages");

            migrationBuilder.DropColumn(
                name: "KitchenServicesid",
                table: "RequestsImages");

            migrationBuilder.DropColumn(
                name: "RequestsCommonPartsid",
                table: "RequestsImages");

            migrationBuilder.DropColumn(
                name: "TimeElapsed",
                table: "RequestCommonParts");

            migrationBuilder.DropColumn(
                name: "RequestImage",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropColumn(
                name: "hasGuarantee",
                table: "ApartmentServicesRequests");

            migrationBuilder.RenameColumn(
                name: "TimeElapsed",
                table: "KitchenServices",
                newName: "RequestImage");

            migrationBuilder.AlterColumn<string>(
                name: "RequestImage",
                table: "RequestCommonParts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TimeElapsed",
                table: "Emergencies",
                type: "time",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
