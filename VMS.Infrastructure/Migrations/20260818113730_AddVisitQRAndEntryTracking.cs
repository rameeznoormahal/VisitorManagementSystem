using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitQRAndEntryTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDate",
                table: "VisitRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutDate",
                table: "VisitRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedInByUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedOutByUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRGeneratedByUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QRGeneratedDate",
                table: "VisitRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRTokenHash",
                table: "VisitRequests",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_CheckedInByUserId",
                table: "VisitRequests",
                column: "CheckedInByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_CheckedOutByUserId",
                table: "VisitRequests",
                column: "CheckedOutByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_QRGeneratedByUserId",
                table: "VisitRequests",
                column: "QRGeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_QRTokenHash",
                table: "VisitRequests",
                column: "QRTokenHash",
                unique: true,
                filter: "[QRTokenHash] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_CheckedInByUserId",
                table: "VisitRequests",
                column: "CheckedInByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_CheckedOutByUserId",
                table: "VisitRequests",
                column: "CheckedOutByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_QRGeneratedByUserId",
                table: "VisitRequests",
                column: "QRGeneratedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_CheckedInByUserId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_CheckedOutByUserId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_QRGeneratedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_CheckedInByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_CheckedOutByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_QRGeneratedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_QRTokenHash",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "CheckedInByUserId",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "CheckedOutByUserId",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "QRGeneratedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "QRGeneratedDate",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "QRTokenHash",
                table: "VisitRequests");
        }
    }
}
