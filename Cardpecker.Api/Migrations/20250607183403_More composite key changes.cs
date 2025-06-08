using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cardpecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class Morecompositekeychanges : Migration
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
                columns: new[] { "ScryfallId", "PricingProvider", "PrintingVersion", "IsMagicOnline" });
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
                columns: new[] { "ScryfallId", "PricingProvider", "PrintingVersion" });
        }
    }
}
