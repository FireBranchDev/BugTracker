using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultProjectRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultProjectRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultProjectRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultProjectRoleProject",
                columns: table => new
                {
                    DefaultProjectRolesId = table.Column<int>(type: "int", nullable: false),
                    ProjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultProjectRoleProject", x => new { x.DefaultProjectRolesId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProject_DefaultProjectRoles_DefaultProjectRolesId",
                        column: x => x.DefaultProjectRolesId,
                        principalTable: "DefaultProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultProjectRoleProjectPermission",
                columns: table => new
                {
                    DefaultProjectRolesId = table.Column<int>(type: "int", nullable: false),
                    ProjectPermissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultProjectRoleProjectPermission", x => new { x.DefaultProjectRolesId, x.ProjectPermissionsId });
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectPermission_DefaultProjectRoles_DefaultProjectRolesId",
                        column: x => x.DefaultProjectRolesId,
                        principalTable: "DefaultProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectPermission_ProjectPermissions_ProjectPermissionsId",
                        column: x => x.ProjectPermissionsId,
                        principalTable: "ProjectPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultProjectRoleProjectUsers",
                columns: table => new
                {
                    DefaultProjectRoleId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultProjectRoleProjectUsers", x => new { x.DefaultProjectRoleId, x.ProjectId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectUsers_DefaultProjectRoles_DefaultProjectRoleId",
                        column: x => x.DefaultProjectRoleId,
                        principalTable: "DefaultProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProjectRoleProject_ProjectsId",
                table: "DefaultProjectRoleProject",
                column: "ProjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProjectRoleProjectPermission_ProjectPermissionsId",
                table: "DefaultProjectRoleProjectPermission",
                column: "ProjectPermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProjectRoleProjectUsers_ProjectId",
                table: "DefaultProjectRoleProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProjectRoleProjectUsers_UserId",
                table: "DefaultProjectRoleProjectUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultProjectRoleProject");

            migrationBuilder.DropTable(
                name: "DefaultProjectRoleProjectPermission");

            migrationBuilder.DropTable(
                name: "DefaultProjectRoleProjectUsers");

            migrationBuilder.DropTable(
                name: "DefaultProjectRoles");
        }
    }
}
