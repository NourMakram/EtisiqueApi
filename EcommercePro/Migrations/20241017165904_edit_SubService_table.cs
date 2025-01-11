using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class edit_SubService_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentSubServicesRequests_ApartmentServicesRequests_RequestId",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentSubServicesRequests_RequestId",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.AddColumn<int>(
                name: "ApartmentServicesRequestid",
                table: "ApartmentSubServicesRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentSubServicesRequests_ApartmentServicesRequestid",
                table: "ApartmentSubServicesRequests",
                column: "ApartmentServicesRequestid");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentSubServicesRequests_ApartmentServicesRequests_ApartmentServicesRequestid",
                table: "ApartmentSubServicesRequests",
                column: "ApartmentServicesRequestid",
                principalTable: "ApartmentServicesRequests",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentSubServicesRequests_ApartmentServicesRequests_ApartmentServicesRequestid",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentSubServicesRequests_ApartmentServicesRequestid",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.DropColumn(
                name: "ApartmentServicesRequestid",
                table: "ApartmentSubServicesRequests");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentSubServicesRequests_RequestId",
                table: "ApartmentSubServicesRequests",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentSubServicesRequests_ApartmentServicesRequests_RequestId",
                table: "ApartmentSubServicesRequests",
                column: "RequestId",
                principalTable: "ApartmentServicesRequests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
