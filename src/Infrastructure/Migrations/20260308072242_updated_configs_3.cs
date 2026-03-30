using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updated_configs_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "two_hundred_day_average_close",
                schema: "candles",
                table: "calculated_price_candles",
                newName: "ma200");

            migrationBuilder.RenameColumn(
                name: "fifty_day_average_close",
                schema: "candles",
                table: "calculated_price_candles",
                newName: "ma50");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ma50",
                schema: "candles",
                table: "calculated_price_candles",
                newName: "fifty_day_average_close");

            migrationBuilder.RenameColumn(
                name: "ma200",
                schema: "candles",
                table: "calculated_price_candles",
                newName: "two_hundred_day_average_close");
        }
    }
}
