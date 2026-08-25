using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkAhead.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaTypeToMonitoredArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaType",
                table: "monitored_areas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                // "" isn't a valid AreaType name — the string<->enum converter would throw
                // reading these rows back. "Other" is a safe default for pre-existing areas.
                defaultValue: "Other");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaType",
                table: "monitored_areas");
        }
    }
}
