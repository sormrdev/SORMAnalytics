using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updated_db_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calculated_price_candles",
                schema: "candles");

            migrationBuilder.AddColumn<decimal>(
                name: "ma200",
                schema: "candles",
                table: "price_candles",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ma50",
                schema: "candles",
                table: "price_candles",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ma200",
                schema: "candles",
                table: "price_candles");

            migrationBuilder.DropColumn(
                name: "ma50",
                schema: "candles",
                table: "price_candles");

            migrationBuilder.CreateTable(
                name: "calculated_price_candles",
                schema: "candles",
                columns: table => new
                {
                    calculated_price_candle_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    price_candle_id = table.Column<int>(type: "integer", nullable: false),
                    ma200 = table.Column<decimal>(type: "numeric", nullable: true),
                    ma50 = table.Column<decimal>(type: "numeric", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "ix_calculated_price_candles_price_candle_id",
                schema: "candles",
                table: "calculated_price_candles",
                column: "price_candle_id",
                unique: true);
        }
    }
}
