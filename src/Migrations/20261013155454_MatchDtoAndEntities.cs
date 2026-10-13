using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnetMVP.Migrations
{
    /// <inheritdoc />
    public partial class MatchDtoAndEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Challenges");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "WriteUps",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Challenges",
                newName: "ShortDescription");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Challenges",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Challenges");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WriteUps",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Challenges",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Challenges",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
