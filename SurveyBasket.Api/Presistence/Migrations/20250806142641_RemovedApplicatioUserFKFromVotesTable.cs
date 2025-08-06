using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedApplicatioUserFKFromVotesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_AspNetUsers_UserId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_PollId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_PollId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Votes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-01652b09f507",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJQ4b0YXzu7WWyv21XYnn6pD4rgqOv2X6Rtg9QmHyNIBivQukyQoVI2HCfyP1Tu/kw==");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PollId",
                table: "Votes",
                column: "PollId",
                unique: true);

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

            migrationBuilder.DropIndex(
                name: "IX_Votes_PollId",
                table: "Votes");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Votes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0198130d-29af-7543-b04f-01652b09f507",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEO+TyjJcYmz06Y1RlG+RXio6ciQbKg86HYZ8Ge3rq4oyrKGKMbQR0wlADt6a+ZSm4A==");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PollId",
                table: "Votes",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_PollId",
                table: "Votes",
                columns: new[] { "UserId", "PollId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_AspNetUsers_UserId",
                table: "Votes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
