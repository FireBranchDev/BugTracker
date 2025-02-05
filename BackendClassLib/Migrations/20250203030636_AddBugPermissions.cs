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
                name: "BugPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugPermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BugPermissionUser",
                columns: table => new
                {
                    BugId = table.Column<int>(type: "int", nullable: false),
                    BugPermissionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugPermissionUser", x => new { x.BugId, x.BugPermissionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_BugPermissionUser_BugPermission_BugPermissionId",
                        column: x => x.BugPermissionId,
                        principalTable: "BugPermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugPermissionUser_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugPermissionUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BugPermission",
                columns: new[] { "Id", "CreatedAt", "Type" },
                values: new object[] { 1, new DateTime(2025, 2, 3, 3, 6, 36, 299, DateTimeKind.Utc).AddTicks(7945), 0 });

            migrationBuilder.CreateIndex(
                name: "IX_BugPermissionUser_BugPermissionId",
                table: "BugPermissionUser",
                column: "BugPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_BugPermissionUser_UserId",
                table: "BugPermissionUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugPermissionUser");

            migrationBuilder.DropTable(
                name: "BugPermission");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BugAssignees",
                newName: "CreatedOn");
        }
    }
}
