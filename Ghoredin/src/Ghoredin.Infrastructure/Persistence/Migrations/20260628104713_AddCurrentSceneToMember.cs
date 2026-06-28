using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ghoredin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentSceneToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentSceneNoteId",
                table: "CampaignMembers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSceneNoteId",
                table: "CampaignMembers");
        }
    }
}
