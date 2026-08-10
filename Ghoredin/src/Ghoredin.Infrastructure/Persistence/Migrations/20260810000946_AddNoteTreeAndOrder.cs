using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ghoredin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteTreeAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentNoteId",
                table: "CampaignNotes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "CampaignNotes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentNoteId",
                table: "CampaignNotes");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "CampaignNotes");
        }
    }
}
