using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cardpecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class EvenMorecompositekeychanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints",
                columns: new[] { "ScryfallId", "PricingProvider", "PrintingVersion", "IsMagicOnline", "Currency" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints",
                columns: new[] { "ScryfallId", "PricingProvider", "PrintingVersion", "IsMagicOnline" });
        }
    }
}
