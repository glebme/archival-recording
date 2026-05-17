using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DevelopmentProposalScrapper.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevelopmentApplications",
                columns: table => new
                {
                    PlanningPortalApplicationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateLastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeterminationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ApplicationStatus = table.Column<int>(type: "integer", nullable: true),
                    ApplicationType = table.Column<int>(type: "integer", nullable: true),
                    Council_CouncilName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevelopmentApplications", x => x.PlanningPortalApplicationNumber);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    DevelopmentApplicationPlanningPortalApplicationNumber = table.Column<string>(type: "character varying(50)", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    X = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Y = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreetNumber1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreetNumber2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreetNumber3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreetName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StreetType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Suburb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Postcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => new { x.DevelopmentApplicationPlanningPortalApplicationNumber, x.Id });
                    table.ForeignKey(
                        name: "FK_Address_DevelopmentApplications_DevelopmentApplicationPlann~",
                        column: x => x.DevelopmentApplicationPlanningPortalApplicationNumber,
                        principalTable: "DevelopmentApplications",
                        principalColumn: "PlanningPortalApplicationNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProposedDevelopmentType",
                columns: table => new
                {
                    DevelopmentApplicationPlanningPortalApplicationNumber = table.Column<string>(type: "character varying(50)", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DevelopmentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposedDevelopmentType", x => new { x.DevelopmentApplicationPlanningPortalApplicationNumber, x.Id });
                    table.ForeignKey(
                        name: "FK_ProposedDevelopmentType_DevelopmentApplications_Development~",
                        column: x => x.DevelopmentApplicationPlanningPortalApplicationNumber,
                        principalTable: "DevelopmentApplications",
                        principalColumn: "PlanningPortalApplicationNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lot",
                columns: table => new
                {
                    AddressDevelopmentApplicationPlanningPortalApplicationNumber = table.Column<string>(type: "character varying(50)", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PlanLabel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lot", x => new { x.AddressDevelopmentApplicationPlanningPortalApplicationNumber, x.AddressId, x.Id });
                    table.ForeignKey(
                        name: "FK_Lot_Address_AddressDevelopmentApplicationPlanningPortalAppl~",
                        columns: x => new { x.AddressDevelopmentApplicationPlanningPortalApplicationNumber, x.AddressId },
                        principalTable: "Address",
                        principalColumns: new[] { "DevelopmentApplicationPlanningPortalApplicationNumber", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lot");

            migrationBuilder.DropTable(
                name: "ProposedDevelopmentType");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "DevelopmentApplications");
        }
    }
}
