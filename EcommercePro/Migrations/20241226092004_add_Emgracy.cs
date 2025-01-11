using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class add_Emgracy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmergencyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EmergencyTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "انقطاع ماء" },
                    { 2, "توقف مصعد" },
                    { 3, "انقطاع كهرباء" },
                    { 4, "التماس كهرباء" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emergencies_EmergencyId",
                table: "Emergencies",
                column: "EmergencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emergencies_EmergencyTypes_EmergencyId",
                table: "Emergencies",
                column: "EmergencyId",
                principalTable: "EmergencyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emergencies_EmergencyTypes_EmergencyId",
                table: "Emergencies");

            migrationBuilder.DropTable(
                name: "EmergencyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Emergencies_EmergencyId",
                table: "Emergencies");
        }
    }
}
