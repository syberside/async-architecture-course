using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace aTES.Billing.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    publicid = table.Column<string>(name: "public-id", type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    publicid = table.Column<string>(name: "public-id", type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    assigned_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    JiraId = table.Column<string>(type: "text", nullable: false),
                    BirdInCageCost = table.Column<int>(type: "integer", nullable: false),
                    MilletInABowlCost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_users_assigned_user_id",
                        column: x => x.assigned_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit = table.Column<long>(type: "bigint", nullable: false),
                    debit = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    details = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_log_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tasks_assigned_user_id",
                table: "tasks",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_log_owner_id",
                table: "transaction_log",
                column: "owner_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "transaction_log");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
