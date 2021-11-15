using Microsoft.EntityFrameworkCore.Migrations;

namespace aTES.TaskTracker.Migrations
{
    public partial class TaskIdSequenceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "tasks-jira-id");

            migrationBuilder.AddColumn<int>(
                name: "sequence_value",
                table: "tasks",
                type: "integer",
                nullable: false,
                defaultValueSql: "nextval('\"tasks-jira-id\"')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "tasks-jira-id");

            migrationBuilder.DropColumn(
                name: "sequence_value",
                table: "tasks");
        }
    }
}
