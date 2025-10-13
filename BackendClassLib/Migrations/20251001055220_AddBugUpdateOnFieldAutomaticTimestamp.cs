using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class AddBugUpdateOnFieldAutomaticTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
                CREATE TRIGGER [dbo].[Bugs_UPDATE] ON [dbo].[Bugs]
                    AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

                UPDATE B
                SET UpdatedOn = GETUTCDATE()
                FROM dbo.Bugs AS B
                INNER JOIN INSERTED AS I
                    ON B.Id = I.Id
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
