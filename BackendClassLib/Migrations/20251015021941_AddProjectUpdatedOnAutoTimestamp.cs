using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendClassLib.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectUpdatedOnAutoTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
                CREATE TRIGGER [dbo].[Projects_UPDATE] ON [dbo].[Projects]
                    AFTER UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

                UPDATE P
                SET UpdatedOn = GETUTCDATE()
                FROM dbo.Projects AS P
                INNER JOIN INSERTED AS I
                    ON P.Id = I.Id
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
                DROP TRIGGER IF EXISTS [dbo].[Projects_UPDATE]
            ");
        }
    }
}
