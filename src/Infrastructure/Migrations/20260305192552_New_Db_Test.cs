using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class New_Db_Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "candles");

            migrationBuilder.CreateTable(
                name: "assets_to_fetch",
                schema: "candles",
                columns: table => new
                {
                    symbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assets_to_fetch", x => x.symbol);
                });

            migrationBuilder.CreateTable(
                name: "price_candles",
                schema: "candles",
                columns: table => new
                {
                    price_candle_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    open = table.Column<decimal>(type: "numeric", nullable: false),
                    high = table.Column<decimal>(type: "numeric", nullable: false),
                    low = table.Column<decimal>(type: "numeric", nullable: false),
                    close = table.Column<decimal>(type: "numeric", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    timestamp = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_price_candles", x => x.price_candle_id);
                });

            migrationBuilder.CreateTable(
                name: "calculated_price_candles",
                schema: "candles",
                columns: table => new
                {
                    calculated_price_candle_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fifty_day_average_close = table.Column<decimal>(type: "numeric", nullable: true),
                    two_hundred_day_average_close = table.Column<decimal>(type: "numeric", nullable: true),
                    price_candle_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calculated_price_candles", x => x.calculated_price_candle_id);
                    table.ForeignKey(
                        name: "fk_calculated_price_candles_price_candles_price_candle_id",
                        column: x => x.price_candle_id,
                        principalSchema: "candles",
                        principalTable: "price_candles",
                        principalColumn: "price_candle_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "candles",
                table: "assets_to_fetch",
                column: "symbol",
                value: "IBM");

            migrationBuilder.CreateIndex(
                name: "ix_calculated_price_candles_price_candle_id",
                schema: "candles",
                table: "calculated_price_candles",
                column: "price_candle_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_price_candles_symbol_timestamp",
                schema: "candles",
                table: "price_candles",
                columns: new[] { "symbol", "timestamp" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assets_to_fetch",
                schema: "candles");

            migrationBuilder.DropTable(
                name: "calculated_price_candles",
                schema: "candles");

            migrationBuilder.DropTable(
                name: "price_candles",
                schema: "candles");
        }
    }
}
