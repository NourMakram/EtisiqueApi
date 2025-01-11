using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtisiqueApi.Migrations
{
    /// <inheritdoc />
    public partial class updatecontract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_projectId",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_projectId",
                table: "Contracts",
                column: "projectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_projectId",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_projectId",
                table: "Contracts",
                column: "projectId",
                unique: true);
        }
    }
}
