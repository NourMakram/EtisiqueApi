using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class updatetableCommonParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestCommonParts_Customers_ManagerId",
                table: "RequestCommonParts");

            migrationBuilder.DropColumn(
                name: "CloseCode",
                table: "RequestCommonParts");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "RequestCommonParts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCommonParts_AspNetUsers_ManagerId",
                table: "RequestCommonParts",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestCommonParts_AspNetUsers_ManagerId",
                table: "RequestCommonParts");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "RequestCommonParts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloseCode",
                table: "RequestCommonParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCommonParts_Customers_ManagerId",
                table: "RequestCommonParts",
                column: "ManagerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
