using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nuevaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DeliveryTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "nvarchar(25)",
                value: "Dine in");

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In progress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DeliveryTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "nvarchar(25)",
                value: "Dine-In");

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In Progress");
        }
    }
}
