using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caro.Data.Migrations
{
    /// <inheritdoc />
    public partial class userr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [Security].[UserRoles] (UserId , RoleId) SELECT '2cfb93f4-c78f-49d6-91f9-67c6360841c6' ,'32b1927e-28d6-4397-aa23-37f9226914c7'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE From [Security].[UserRoles] WHERE UserId = '2cfb93f4-c78f-49d6-91f9-67c6360841c6'");
        }
    }
}
