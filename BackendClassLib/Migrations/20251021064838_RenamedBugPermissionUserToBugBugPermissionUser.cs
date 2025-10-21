using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class RenamedBugPermissionUserToBugBugPermissionUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugPermissionUsers");

            migrationBuilder.CreateTable(
                name: "BugBugPermissionUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [BaseSequence]"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BugId = table.Column<int>(type: "int", nullable: false),
                    BugPermissionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugBugPermissionUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugBugPermissionUsers_BugPermissions_BugPermissionId",
                        column: x => x.BugPermissionId,
                        principalTable: "BugPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugBugPermissionUsers_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugBugPermissionUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugBugPermissionUsers_BugId",
                table: "BugBugPermissionUsers",
                column: "BugId");

            migrationBuilder.CreateIndex(
                name: "IX_BugBugPermissionUsers_BugPermissionId",
                table: "BugBugPermissionUsers",
                column: "BugPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_BugBugPermissionUsers_UserId",
                table: "BugBugPermissionUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugBugPermissionUsers");

            migrationBuilder.CreateTable(
                name: "BugPermissionUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [BaseSequence]"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BugId = table.Column<int>(type: "int", nullable: false),
                    BugPermissionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugPermissionUsers", x => x.Id);
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
                name: "IX_BugPermissionUsers_BugId",
                table: "BugPermissionUsers",
                column: "BugId");

            migrationBuilder.CreateIndex(
                name: "IX_BugPermissionUsers_BugPermissionId",
                table: "BugPermissionUsers",
                column: "BugPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_BugPermissionUsers_UserId",
                table: "BugPermissionUsers",
                column: "UserId");
        }
    }
}
