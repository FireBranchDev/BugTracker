using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class ProjectProjectRoleUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectProjectRoleUsers",
                columns: table => new
                {
                    ProjectsId = table.Column<int>(type: "int", nullable: false),
                    ProjectRolesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ProjectRoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProjectRoleUsers", x => new { x.ProjectsId, x.ProjectRolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ProjectProjectRoleUsers_ProjectRoles_ProjectRoleId",
                        column: x => x.ProjectRoleId,
                        principalTable: "ProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProjectRoleUsers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProjectRoleUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProjectRoleUsers_ProjectId",
                table: "ProjectProjectRoleUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProjectRoleUsers_ProjectRoleId",
                table: "ProjectProjectRoleUsers",
                column: "ProjectRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProjectRoleUsers_UserId",
                table: "ProjectProjectRoleUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectProjectRoleUsers");
        }
    }
}
