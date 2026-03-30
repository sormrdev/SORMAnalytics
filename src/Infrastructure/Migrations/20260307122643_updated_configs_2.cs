using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updated_configs_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "candles",
                table: "assets_to_fetch",
                keyColumn: "symbol",
                keyValue: "IBM");

            migrationBuilder.InsertData(
                schema: "candles",
                table: "assets_to_fetch",
                column: "symbol",
                value: "TSLA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "candles",
                table: "assets_to_fetch",
                keyColumn: "symbol",
                keyValue: "TSLA");

            migrationBuilder.InsertData(
                schema: "candles",
                table: "assets_to_fetch",
                column: "symbol",
                value: "IBM");
        }
    }
}
