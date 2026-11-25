using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KbKgeu.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationsAndChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "publication_requests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "New",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddCheckConstraint(
                name: "publication_requests_description_check",
                table: "publication_requests",
                sql: "char_length(description) BETWEEN 10 AND 500");

            migrationBuilder.AddCheckConstraint(
                name: "publication_requests_title_check",
                table: "publication_requests",
                sql: "char_length(title) BETWEEN 5 AND 80");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "publication_requests_description_check",
                table: "publication_requests");

            migrationBuilder.DropCheckConstraint(
                name: "publication_requests_title_check",
                table: "publication_requests");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "publication_requests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "New");
        }
    }
}
