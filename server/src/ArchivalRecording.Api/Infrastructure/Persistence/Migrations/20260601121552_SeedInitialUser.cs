using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchivalRecording.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "AllowedUsers" ("Id", "Email")
                VALUES (gen_random_uuid(), 'thg.truongz@gmail.com')
                ON CONFLICT ("Email") DO NOTHING;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "AllowedUsers" WHERE "Email" = 'thg.truongz@gmail.com';
                """);
        }
    }
}
