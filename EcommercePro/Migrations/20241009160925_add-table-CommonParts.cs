using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class addtableCommonParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestsCommonPartsid",
                table: "ApartmentSubServicesRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestCommonParts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<int>(type: "int", nullable: false),
                    projectId = table.Column<int>(type: "int", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestStuatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfVisit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumDays = table.Column<int>(type: "int", nullable: false),
                    RequestImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    CloseCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCommonParts", x => x.id);
                    table.ForeignKey(
                        name: "FK_RequestCommonParts_ApartmentServices_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ApartmentServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestCommonParts_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestCommonParts_Customers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestCommonParts_Projects_projectId",
                        column: x => x.projectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentSubServicesRequests_RequestsCommonPartsid",
                table: "ApartmentSubServicesRequests",
                column: "RequestsCommonPartsid");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCommonParts_ManagerId",
                table: "RequestCommonParts",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCommonParts_projectId",
                table: "RequestCommonParts",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCommonParts_ServiceTypeId",
                table: "RequestCommonParts",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCommonParts_TechnicianId",
                table: "RequestCommonParts",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentSubServicesRequests_RequestCommonParts_RequestsCommonPartsid",
                table: "ApartmentSubServicesRequests",
                column: "RequestsCommonPartsid",
                principalTable: "RequestCommonParts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentSubServicesRequests_RequestCommonParts_RequestsCommonPartsid",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.DropTable(
                name: "RequestCommonParts");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentSubServicesRequests_RequestsCommonPartsid",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.DropColumn(
                name: "RequestsCommonPartsid",
                table: "ApartmentSubServicesRequests");
        }
    }
}
