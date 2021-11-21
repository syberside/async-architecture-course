using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace aTES.Analytics.Migrations
{
    public partial class Tasks_attributes_updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "fullname",
                table: "Tasks",
                newName: "public_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "closed_at",
                table: "Tasks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closed_at",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "public_id",
                table: "Tasks",
                newName: "fullname");
        }
    }
}
