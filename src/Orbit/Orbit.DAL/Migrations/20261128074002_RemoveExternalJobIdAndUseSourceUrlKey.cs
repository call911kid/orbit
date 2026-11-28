using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbit.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExternalJobIdAndUseSourceUrlKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Source_ExternalJobId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "ExternalJobId",
                table: "JobPostings");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "JobPostings",
                type: "varchar(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Source_Url",
                table: "JobPostings",
                columns: new[] { "Source", "Url" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Source_Url",
                table: "JobPostings");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "JobPostings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldUnicode: false,
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "ExternalJobId",
                table: "JobPostings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Source_ExternalJobId",
                table: "JobPostings",
                columns: new[] { "Source", "ExternalJobId" },
                unique: true);
        }
    }
}
