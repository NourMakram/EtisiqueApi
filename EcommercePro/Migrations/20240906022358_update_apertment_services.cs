using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class update_apertment_services : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentServicesRequests_AspNetUsers_ApplicationUserId",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentServicesRequests_ApplicationUserId",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ApartmentServicesRequests");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                table: "ApartmentServicesRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentServicesRequests_TechnicianId",
                table: "ApartmentServicesRequests",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentServicesRequests_AspNetUsers_TechnicianId",
                table: "ApartmentServicesRequests",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentServicesRequests_AspNetUsers_TechnicianId",
                table: "ApartmentServicesRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentServicesRequests_TechnicianId",
                table: "ApartmentServicesRequests");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                table: "ApartmentServicesRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ApartmentServicesRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentServicesRequests_ApplicationUserId",
                table: "ApartmentServicesRequests",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentServicesRequests_AspNetUsers_ApplicationUserId",
                table: "ApartmentServicesRequests",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
