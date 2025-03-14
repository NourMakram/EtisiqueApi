﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class create_New_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropColumn(
                name: "PostponementDate",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropColumn(
                name: "TransferDate",
                table: "ApartmentServicesRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApartmentServicesRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApartmentServicesRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ApartmentServicesRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostponementDate",
                table: "ApartmentServicesRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferDate",
                table: "ApartmentServicesRequests",
                type: "datetime2",
                nullable: true);
        }
    }
}
