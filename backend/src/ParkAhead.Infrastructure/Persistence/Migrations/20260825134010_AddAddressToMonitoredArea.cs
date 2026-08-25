using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkAhead.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToMonitoredArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "monitored_areas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "monitored_areas");
        }
    }
}
