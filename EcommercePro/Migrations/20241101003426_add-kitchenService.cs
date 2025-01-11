using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class addkitchenService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentServicesRequests_Locations_RequestLocationId",
                table: "ApartmentServicesRequests");

            migrationBuilder.AlterColumn<int>(
                name: "RequestLocationId",
                table: "ApartmentServicesRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "KitchenServices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<int>(type: "int", nullable: false),
                    projectId = table.Column<int>(type: "int", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitNo = table.Column<int>(type: "int", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestLocationId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStuatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumDays = table.Column<int>(type: "int", nullable: false),
                    RequestImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreementFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenServices", x => x.id);
                    table.ForeignKey(
                        name: "FK_KitchenServices_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KitchenServices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KitchenServices_Locations_RequestLocationId",
                        column: x => x.RequestLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KitchenServices_Projects_projectId",
                        column: x => x.projectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KitchenServices_CustomerId",
                table: "KitchenServices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenServices_projectId",
                table: "KitchenServices",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenServices_RequestLocationId",
                table: "KitchenServices",
                column: "RequestLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenServices_TechnicianId",
                table: "KitchenServices",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentServicesRequests_Locations_RequestLocationId",
                table: "ApartmentServicesRequests",
                column: "RequestLocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentServicesRequests_Locations_RequestLocationId",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropTable(
                name: "KitchenServices");

            migrationBuilder.AlterColumn<int>(
                name: "RequestLocationId",
                table: "ApartmentServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentServicesRequests_Locations_RequestLocationId",
                table: "ApartmentServicesRequests",
                column: "RequestLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
