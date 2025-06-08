using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cardpecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Magic");

            migrationBuilder.CreateTable(
                name: "CardInfos",
                schema: "Magic",
                columns: table => new
                {
                    ScryfallId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardInfos", x => x.ScryfallId);
                });

            migrationBuilder.CreateTable(
                name: "PricingPoints",
                schema: "Magic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScryfallId = table.Column<Guid>(type: "uuid", nullable: false),
                    PricingProvider = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    IsMagicOnline = table.Column<bool>(type: "boolean", nullable: false),
                    PrintingVersion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PriceDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingPoints_CardInfos_ScryfallId",
                        column: x => x.ScryfallId,
                        principalSchema: "Magic",
                        principalTable: "CardInfos",
                        principalColumn: "ScryfallId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingPoints_ScryfallId",
                schema: "Magic",
                table: "PricingPoints",
                column: "ScryfallId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingPoints",
                schema: "Magic");

            migrationBuilder.DropTable(
                name: "CardInfos",
                schema: "Magic");
        }
    }
}
