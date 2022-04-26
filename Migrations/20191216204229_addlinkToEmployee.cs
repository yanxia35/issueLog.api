using Microsoft.EntityFrameworkCore.Migrations;

namespace IssueLog.API.Migrations
{
    public partial class addlinkToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_part_issue_resolved_by",
                schema: "issue_log",
                table: "part_issue",
                column: "resolved_by");

            migrationBuilder.AddForeignKey(
                name: "FK_part_issue_employees_resolved_by",
                schema: "issue_log",
                table: "part_issue",
                column: "resolved_by",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_part_issue_employees_resolved_by",
                schema: "issue_log",
                table: "part_issue");

            migrationBuilder.DropIndex(
                name: "IX_part_issue_resolved_by",
                schema: "issue_log",
                table: "part_issue");
        }
    }
}
