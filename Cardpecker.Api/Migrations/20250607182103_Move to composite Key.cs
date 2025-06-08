using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cardpecker.Api.Migrations
{
    /// <inheritdoc />
    public partial class MovetocompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Magic",
                table: "PricingPoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints",
                columns: new[] { "ScryfallId", "PricingProvider", "PrintingVersion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "Magic",
                table: "PricingPoints",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricingPoints",
                schema: "Magic",
                table: "PricingPoints",
                column: "Id");
        }
    }
}
