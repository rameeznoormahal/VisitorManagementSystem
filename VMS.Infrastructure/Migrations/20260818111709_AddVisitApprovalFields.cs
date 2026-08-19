using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DecisionByUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecisionComments",
                table: "VisitRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "VisitRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_DecisionByUserId",
                table: "VisitRequests",
                column: "DecisionByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_DecisionByUserId",
                table: "VisitRequests",
                column: "DecisionByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_DecisionByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_DecisionByUserId",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "DecisionByUserId",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "DecisionComments",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "VisitRequests");
        }
    }
}
