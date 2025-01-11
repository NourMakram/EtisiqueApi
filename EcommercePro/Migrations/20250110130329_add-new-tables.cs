using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class addnewtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumDays",
                table: "RequestCommonParts");

            migrationBuilder.DropColumn(
                name: "NumDays",
                table: "KitchenServices");

            migrationBuilder.DropColumn(
                name: "NumDays",
                table: "ApartmentServicesRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmation",
                table: "ApartmentServicesRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApartmentServicesVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproverID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeElapsed = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentServicesVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApartmentServicesVerifications_ApartmentServicesRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ApartmentServicesRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApartmentServicesVerifications_AspNetUsers_ApproverID",
                        column: x => x.ApproverID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommonPartsVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproverID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeElapsed = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonPartsVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonPartsVerifications_AspNetUsers_ApproverID",
                        column: x => x.ApproverID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonPartsVerifications_RequestCommonParts_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestCommonParts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentServicesVerifications_ApproverID",
                table: "ApartmentServicesVerifications",
                column: "ApproverID");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentServicesVerifications_RequestId",
                table: "ApartmentServicesVerifications",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommonPartsVerifications_ApproverID",
                table: "CommonPartsVerifications",
                column: "ApproverID");

            migrationBuilder.CreateIndex(
                name: "IX_CommonPartsVerifications_RequestId",
                table: "CommonPartsVerifications",
                column: "RequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmentServicesVerifications");

            migrationBuilder.DropTable(
                name: "CommonPartsVerifications");

            migrationBuilder.DropColumn(
                name: "IsConfirmation",
                table: "ApartmentServicesRequests");

            migrationBuilder.AddColumn<int>(
                name: "NumDays",
                table: "RequestCommonParts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumDays",
                table: "KitchenServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumDays",
                table: "ApartmentServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
