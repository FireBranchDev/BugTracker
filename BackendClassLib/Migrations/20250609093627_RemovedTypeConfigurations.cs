using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTypeConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugAssignees_Bugs_BugId",
                table: "BugAssignees");

            migrationBuilder.DropForeignKey(
                name: "FK_BugAssignees_Users_UserId",
                table: "BugAssignees");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultProjectRoleProjectUsers_DefaultProjectRoles_DefaultProjectRoleId",
                table: "DefaultProjectRoleProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultProjectRoleProjectUsers_Projects_ProjectId",
                table: "DefaultProjectRoleProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultProjectRoleProjectUsers_Users_UserId",
                table: "DefaultProjectRoleProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_Projects_ProjectsId",
                table: "ProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_Users_UsersId",
                table: "ProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProjectPermissions",
                table: "UserProjectPermissions");

            migrationBuilder.DropIndex(
                name: "IX_ProjectPermissions_Type",
                table: "ProjectPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BugPermissionUsers",
                table: "BugPermissionUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultProjectRoleProjectUsers",
                table: "DefaultProjectRoleProjectUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BugAssignees",
                table: "BugAssignees");

            migrationBuilder.RenameTable(
                name: "DefaultProjectRoleProjectUsers",
                newName: "DefaultProjectRoleProjectUser");

            migrationBuilder.RenameTable(
                name: "BugAssignees",
                newName: "BugAssignee");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "ProjectUser",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "ProjectUser",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUser_UsersId",
                table: "ProjectUser",
                newName: "IX_ProjectUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultProjectRoleProjectUsers_UserId",
                table: "DefaultProjectRoleProjectUser",
                newName: "IX_DefaultProjectRoleProjectUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultProjectRoleProjectUsers_ProjectId",
                table: "DefaultProjectRoleProjectUser",
                newName: "IX_DefaultProjectRoleProjectUser_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_BugAssignees_UserId",
                table: "BugAssignee",
                newName: "IX_BugAssignee_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserProjectPermissions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Joined",
                table: "ProjectUser",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DefaultProjectRoleProjectUser",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProjectPermissions",
                table: "UserProjectPermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BugPermissionUsers",
                table: "BugPermissionUsers",
                columns: new[] { "BugId", "BugPermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultProjectRoleProjectUser",
                table: "DefaultProjectRoleProjectUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BugAssignee",
                table: "BugAssignee",
                columns: new[] { "BugId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_UserId",
                table: "UserProjectPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProjectRoleProjectUser_DefaultProjectRoleId",
                table: "DefaultProjectRoleProjectUser",
                column: "DefaultProjectRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugAssignee_Bugs_BugId",
                table: "BugAssignee",
                column: "BugId",
                principalTable: "Bugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BugAssignee_Users_UserId",
                table: "BugAssignee",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultProjectRoleProjectUser_DefaultProjectRoles_DefaultProjectRoleId",
                table: "DefaultProjectRoleProjectUser",
                column: "DefaultProjectRoleId",
                principalTable: "DefaultProjectRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultProjectRoleProjectUser_Projects_ProjectId",
                table: "DefaultProjectRoleProjectUser",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultProjectRoleProjectUser_Users_UserId",
                table: "DefaultProjectRoleProjectUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_Projects_ProjectId",
                table: "ProjectUser",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_Users_UserId",
                table: "ProjectUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BugAssignee_Bugs_BugId",
                table: "BugAssignee");

            migrationBuilder.DropForeignKey(
                name: "FK_BugAssignee_Users_UserId",
                table: "BugAssignee");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultProjectRoleProjectUser_DefaultProjectRoles_DefaultProjectRoleId",
                table: "DefaultProjectRoleProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultProjectRoleProjectUser_Projects_ProjectId",
                table: "DefaultProjectRoleProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultProjectRoleProjectUser_Users_UserId",
                table: "DefaultProjectRoleProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_Projects_ProjectId",
                table: "ProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_Users_UserId",
                table: "ProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProjectPermissions",
                table: "UserProjectPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserProjectPermissions_UserId",
                table: "UserProjectPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BugPermissionUsers",
                table: "BugPermissionUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultProjectRoleProjectUser",
                table: "DefaultProjectRoleProjectUser");

            migrationBuilder.DropIndex(
                name: "IX_DefaultProjectRoleProjectUser_DefaultProjectRoleId",
                table: "DefaultProjectRoleProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BugAssignee",
                table: "BugAssignee");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserProjectPermissions");

            migrationBuilder.DropColumn(
                name: "Joined",
                table: "ProjectUser");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DefaultProjectRoleProjectUser");

            migrationBuilder.RenameTable(
                name: "DefaultProjectRoleProjectUser",
                newName: "DefaultProjectRoleProjectUsers");

            migrationBuilder.RenameTable(
                name: "BugAssignee",
                newName: "BugAssignees");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProjectUser",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "ProjectUser",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUser_UserId",
                table: "ProjectUser",
                newName: "IX_ProjectUser_UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultProjectRoleProjectUser_UserId",
                table: "DefaultProjectRoleProjectUsers",
                newName: "IX_DefaultProjectRoleProjectUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultProjectRoleProjectUser_ProjectId",
                table: "DefaultProjectRoleProjectUsers",
                newName: "IX_DefaultProjectRoleProjectUsers_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_BugAssignee_UserId",
                table: "BugAssignees",
                newName: "IX_BugAssignees_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProjectPermissions",
                table: "UserProjectPermissions",
                columns: new[] { "UserId", "ProjectId", "ProjectPermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BugPermissionUsers",
                table: "BugPermissionUsers",
                columns: new[] { "BugId", "BugPermissionId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultProjectRoleProjectUsers",
                table: "DefaultProjectRoleProjectUsers",
                columns: new[] { "DefaultProjectRoleId", "ProjectId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BugAssignees",
                table: "BugAssignees",
                columns: new[] { "BugId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPermissions_Type",
                table: "ProjectPermissions",
                column: "Type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BugAssignees_Bugs_BugId",
                table: "BugAssignees",
                column: "BugId",
                principalTable: "Bugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BugAssignees_Users_UserId",
                table: "BugAssignees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultProjectRoleProjectUsers_DefaultProjectRoles_DefaultProjectRoleId",
                table: "DefaultProjectRoleProjectUsers",
                column: "DefaultProjectRoleId",
                principalTable: "DefaultProjectRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultProjectRoleProjectUsers_Projects_ProjectId",
                table: "DefaultProjectRoleProjectUsers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultProjectRoleProjectUsers_Users_UserId",
                table: "DefaultProjectRoleProjectUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_Projects_ProjectsId",
                table: "ProjectUser",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_Users_UsersId",
                table: "ProjectUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
