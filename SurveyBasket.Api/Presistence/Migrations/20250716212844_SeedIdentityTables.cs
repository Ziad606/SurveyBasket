using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0198130d-29af-7543-b04f-01670b914e2c", "0198130d-29af-7543-b04f-016856f9f854", false, false, "Admin", "ADMIN" },
                    { "0198130d-29af-7543-b04f-0169e0118da3", "0198130d-29af-7543-b04f-016a5c075f37", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0198130d-29af-7543-b04f-01652b09f507", 0, "0198130d-29af-7543-b04f-0166c2a2dbb1", "admin@survey-basket.com", true, "Ziad", "Mohammed", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEECbLu5+fG0piDRqlcMzB4JEcqhfSpgJK/7sylPBF6MF+tI2DDatNWRjn+LYSiF5dQ==", null, false, "822133515D3340F3AEDF6022D5ECBB51", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "persmissions", "polls:read", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 2, "persmissions", "polls:add", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 3, "persmissions", "polls:update", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 4, "persmissions", "polls:delete", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 5, "persmissions", "questions:read", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 6, "persmissions", "questions:add", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 7, "persmissions", "questions:update", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 8, "persmissions", "users:read", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 9, "persmissions", "users:add", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 10, "persmissions", "users:update", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 11, "persmissions", "roles:read", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 12, "persmissions", "roles:add", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 13, "persmissions", "roles:update", "0198130d-29af-7543-b04f-01670b914e2c" },
                    { 14, "persmissions", "results:read", "0198130d-29af-7543-b04f-01670b914e2c" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0198130d-29af-7543-b04f-01670b914e2c", "0198130d-29af-7543-b04f-01652b09f507" });

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-0169e0118da3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0198130d-29af-7543-b04f-01670b914e2c", "0198130d-29af-7543-b04f-01652b09f507" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-01670b914e2c");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-01652b09f507");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
