using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class AddBugPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "BugAssignees",
                newName: "CreatedAt");

            migrationBuilder.CreateTable(
                name: "BugPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BugPermissionUsers",
                columns: table => new
                {
                    BugId = table.Column<int>(type: "int", nullable: false),
                    BugPermissionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugPermissionUsers", x => new { x.BugId, x.BugPermissionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_BugPermissionUsers_BugPermissions_BugPermissionId",
                        column: x => x.BugPermissionId,
                        principalTable: "BugPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugPermissionUsers_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugPermissionUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugPermissionUsers_BugPermissionId",
                table: "BugPermissionUsers",
                column: "BugPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_BugPermissionUsers_UserId",
                table: "BugPermissionUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugPermissionUsers");

            migrationBuilder.DropTable(
                name: "BugPermissions");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BugAssignees",
                newName: "CreatedOn");
        }
    }
}
