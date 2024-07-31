using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caro.Data.Migrations
{
    /// <inheritdoc />
    public partial class userroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [Security].[UserRoles] (UserId , RoleId) SELECT '6e655b30-000f-4ef3-a791-bffe1cbf1061' , Id FROM [Security].[Roles]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE From [Security].[UserRoles] WHERE UserId = '6e655b30-000f-4ef3-a791-bffe1cbf1061'");
        }
    }
}
