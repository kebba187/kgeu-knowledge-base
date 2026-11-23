using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KbKgeu.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_publication_requests_created_by_user_id",
                table: "publication_requests",
                newName: "publication_requests_created_by_user_id_idx");

            migrationBuilder.CreateIndex(
                name: "publication_requests_created_at_idx",
                table: "publication_requests",
                column: "created_at",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "publication_requests_created_at_idx",
                table: "publication_requests");

            migrationBuilder.RenameIndex(
                name: "publication_requests_created_by_user_id_idx",
                table: "publication_requests",
                newName: "IX_publication_requests_created_by_user_id");
        }
    }
}
