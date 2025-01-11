using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class add_Emergency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReceived",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedCode",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeElapsed",
                table: "ApartmentServicesRequests",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attchments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestManagementId = table.Column<int>(type: "int", nullable: false),
                    imagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attchments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attchments_RequestManagements_RequestManagementId",
                        column: x => x.RequestManagementId,
                        principalTable: "RequestManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Emergencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<int>(type: "int", nullable: false),
                    projectId = table.Column<int>(type: "int", nullable: false),
                    EmergencyId = table.Column<int>(type: "int", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitNo = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStuatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeElapsed = table.Column<TimeSpan>(type: "time", nullable: true),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CloseCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emergencies", x => x.id);
                    table.ForeignKey(
                        name: "FK_Emergencies_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Emergencies_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Emergencies_Projects_projectId",
                        column: x => x.projectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestsImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    imagepath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emergencyid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestsImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestsImages_Emergencies_Emergencyid",
                        column: x => x.Emergencyid,
                        principalTable: "Emergencies",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attchments_RequestManagementId",
                table: "Attchments",
                column: "RequestManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Emergencies_CustomerId",
                table: "Emergencies",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Emergencies_projectId",
                table: "Emergencies",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_Emergencies_TechnicianId",
                table: "Emergencies",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestsImages_Emergencyid",
                table: "RequestsImages",
                column: "Emergencyid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attchments");

            migrationBuilder.DropTable(
                name: "RequestsImages");

            migrationBuilder.DropTable(
                name: "Emergencies");

            migrationBuilder.DropColumn(
                name: "IsReceived",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ReceivedCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TimeElapsed",
                table: "ApartmentServicesRequests");
        }
    }
}
