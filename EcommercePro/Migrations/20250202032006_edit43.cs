using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class edit43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApartmentServices",
                columns: new[] { "Id", "Name" },
                values: new object[] { 10, "نظافة" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
