using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedPermissionsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14,
                column: "ClaimType",
                value: "permissions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-01652b09f507",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKzZso9YgS5FpShR/9C8VqUAZhnaIinHtMefTxmA/9FzpxwGIAZVpiH65qTCz5I4eA==");

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

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14,
                column: "ClaimType",
                value: "persmissions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-01652b09f507",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEECbLu5+fG0piDRqlcMzB4JEcqhfSpgJK/7sylPBF6MF+tI2DDatNWRjn+LYSiF5dQ==");

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
