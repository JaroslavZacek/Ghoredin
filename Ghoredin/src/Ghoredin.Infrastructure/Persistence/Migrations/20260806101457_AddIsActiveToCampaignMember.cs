using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ghoredin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToCampaignMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CampaignMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CampaignMembers");
        }
    }
}
