using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFHSocial.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserHasProfilePic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasProfilePicture",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasProfilePicture",
                table: "AspNetUsers");
        }
    }
}
