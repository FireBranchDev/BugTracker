﻿// <auto-generated />
using System;
using BackendClassLib.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackendClassLib.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BackendClassLib.Database.Models.Auth", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.PrimitiveCollection<string>("UserIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Auths");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.Bug", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(90)
                        .HasColumnType("nvarchar(90)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Bugs");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.BugAssignee", b =>
                {
                    b.Property<int>("BugId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("BugId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("BugAssignees", (string)null);
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.BugPermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("BugPermissions");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.BugPermissionUser", b =>
                {
                    b.Property<int>("BugId")
                        .HasColumnType("int");

                    b.Property<int>("BugPermissionId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("BugId", "BugPermissionId", "UserId");

                    b.HasIndex("BugPermissionId");

                    b.HasIndex("UserId");

                    b.ToTable("BugPermissionUsers");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.DefaultProjectRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("DefaultProjectRoles");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.DefaultProjectRoleProjectUser", b =>
                {
                    b.Property<int>("DefaultProjectRoleId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("DefaultProjectRoleId", "ProjectId", "UserId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("DefaultProjectRoleProjectUsers", (string)null);
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("nvarchar(120)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.ProjectPermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Type")
                        .IsUnique();

                    b.ToTable("ProjectPermissions");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AuthId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.UserProjectPermission", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectPermissionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "ProjectId", "ProjectPermissionId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ProjectPermissionId");

                    b.ToTable("UserProjectPermissions");
                });

            modelBuilder.Entity("DefaultProjectRoleProject", b =>
                {
                    b.Property<int>("DefaultProjectRolesId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectsId")
                        .HasColumnType("int");

                    b.HasKey("DefaultProjectRolesId", "ProjectsId");

                    b.HasIndex("ProjectsId");

                    b.ToTable("DefaultProjectRoleProject");
                });

            modelBuilder.Entity("DefaultProjectRoleProjectPermission", b =>
                {
                    b.Property<int>("DefaultProjectRolesId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectPermissionsId")
                        .HasColumnType("int");

                    b.HasKey("DefaultProjectRolesId", "ProjectPermissionsId");

                    b.HasIndex("ProjectPermissionsId");

                    b.ToTable("DefaultProjectRoleProjectPermission");
                });

            modelBuilder.Entity("ProjectUser", b =>
                {
                    b.Property<int>("ProjectsId")
                        .HasColumnType("int");

                    b.Property<int>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("ProjectsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ProjectUser");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.Bug", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.Project", "Project")
                        .WithMany("Bugs")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.BugAssignee", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.Bug", "Bug")
                        .WithMany("BugAssignees")
                        .HasForeignKey("BugId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.User", "User")
                        .WithMany("BugAssignees")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bug");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.BugPermissionUser", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.Bug", null)
                        .WithMany("BugPermissionUsers")
                        .HasForeignKey("BugId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.BugPermission", null)
                        .WithMany("BugPermissionUsers")
                        .HasForeignKey("BugPermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.User", null)
                        .WithMany("BugPermissionUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.DefaultProjectRoleProjectUser", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.DefaultProjectRole", "DefaultProjectRole")
                        .WithMany("DefaultProjectRoleProjectUsers")
                        .HasForeignKey("DefaultProjectRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.Project", "Project")
                        .WithMany("DefaultProjectRoleProjectUsers")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.User", "User")
                        .WithMany("DefaultProjectRoleProjectUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DefaultProjectRole");

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.User", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.Auth", "Auth")
                        .WithOne("User")
                        .HasForeignKey("BackendClassLib.Database.Models.User", "AuthId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auth");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.UserProjectPermission", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.Project", "Project")
                        .WithMany("UserProjectPermissions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.ProjectPermission", "ProjectPermission")
                        .WithMany("UserProjectPermissions")
                        .HasForeignKey("ProjectPermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.User", "User")
                        .WithMany("UserProjectPermissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("ProjectPermission");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DefaultProjectRoleProject", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.DefaultProjectRole", null)
                        .WithMany()
                        .HasForeignKey("DefaultProjectRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DefaultProjectRoleProjectPermission", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.DefaultProjectRole", null)
                        .WithMany()
                        .HasForeignKey("DefaultProjectRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.ProjectPermission", null)
                        .WithMany()
                        .HasForeignKey("ProjectPermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProjectUser", b =>
                {
                    b.HasOne("BackendClassLib.Database.Models.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendClassLib.Database.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.Auth", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.Bug", b =>
                {
                    b.Navigation("BugAssignees");

                    b.Navigation("BugPermissionUsers");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.BugPermission", b =>
                {
                    b.Navigation("BugPermissionUsers");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.DefaultProjectRole", b =>
                {
                    b.Navigation("DefaultProjectRoleProjectUsers");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.Project", b =>
                {
                    b.Navigation("Bugs");

                    b.Navigation("DefaultProjectRoleProjectUsers");

                    b.Navigation("UserProjectPermissions");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.ProjectPermission", b =>
                {
                    b.Navigation("UserProjectPermissions");
                });

            modelBuilder.Entity("BackendClassLib.Database.Models.User", b =>
                {
                    b.Navigation("BugAssignees");

                    b.Navigation("BugPermissionUsers");

                    b.Navigation("DefaultProjectRoleProjectUsers");

                    b.Navigation("UserProjectPermissions");
                });
#pragma warning restore 612, 618
        }
    }
}
