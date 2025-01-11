using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class shortCut3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.UpdateData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "خشب");

            migrationBuilder.UpdateData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "جبس");

            migrationBuilder.InsertData(
                table: "ApartmentServices",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 8, "نحاس تكييف" },
                    { 9, "انتركوم" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "shorts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.UpdateData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "مكافحة حشرات");

            migrationBuilder.UpdateData(
                table: "ApartmentServices",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "شتر");
        }
    }
}
