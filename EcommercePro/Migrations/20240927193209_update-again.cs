using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class updateagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentServicesRequests_Customers_CustomerId",
                table: "ApartmentServicesRequests");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ApartmentServicesRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentServicesRequests_Customers_CustomerId",
                table: "ApartmentServicesRequests",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentServicesRequests_Customers_CustomerId",
                table: "ApartmentServicesRequests");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ApartmentServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentServicesRequests_Customers_CustomerId",
                table: "ApartmentServicesRequests",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
