using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IssueLog.API.Migrations.Temp
{
    public partial class issuefile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "issue_log");

            
            migrationBuilder.CreateTable(
                name: "issue_files",
                schema: "issue_log",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    file_url = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    issue_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_issue_files_issues_issue_id",
                        column: x => x.issue_id,
                        principalSchema: "issue_log",
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

              migrationBuilder.CreateIndex(
                name: "IX_issue_files_issue_id",
                schema: "issue_log",
                table: "issue_files",
                column: "issue_id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropTable(
                name: "issue_files",
                schema: "issue_log");

        }
    }
}
