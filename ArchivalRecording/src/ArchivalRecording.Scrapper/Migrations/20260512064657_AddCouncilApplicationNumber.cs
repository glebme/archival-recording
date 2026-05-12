using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevelopmentProposalScrapper.Migrations
{
    /// <inheritdoc />
    public partial class AddCouncilApplicationNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouncilApplicationNumber",
                table: "DevelopmentApplications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouncilApplicationNumber",
                table: "DevelopmentApplications");
        }
    }
}
