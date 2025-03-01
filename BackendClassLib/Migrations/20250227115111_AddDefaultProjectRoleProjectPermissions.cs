using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultProjectRoleProjectPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultProjectRoleProjectPermissions",
                columns: table => new
                {
                    DefaultProjectRolesId = table.Column<int>(type: "int", nullable: false),
                    ProjectPermissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultProjectRoleProjectPermissions", x => new { x.DefaultProjectRolesId, x.ProjectPermissionsId });
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectPermissions_DefaultProjectRoles_DefaultProjectRolesId",
                        column: x => x.DefaultProjectRolesId,
                        principalTable: "DefaultProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultProjectRoleProjectPermissions_ProjectPermissions_ProjectPermissionsId",
                        column: x => x.ProjectPermissionsId,
                        principalTable: "ProjectPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProjectRoleProjectPermissions_ProjectPermissionsId",
                table: "DefaultProjectRoleProjectPermissions",
                column: "ProjectPermissionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultProjectRoleProjectPermissions");
        }
    }
}
