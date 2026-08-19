using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVisitForMultipleVisitors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_Visitors_VisitorId",
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

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_Status",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_VisitDate",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_VisitorId",
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
                name: "ExpectedArrivalTime",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "ExpectedDepartureTime",
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

            migrationBuilder.DropColumn(
                name: "QRTokenProtected",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "VisitDate",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "VisitRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitFromDateTime",
                table: "VisitRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitToDateTime",
                table: "VisitRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Visitors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Visitors",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IdExpiryDate",
                table: "Visitors",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "IdType",
                table: "Visitors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Visitors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VisitVisitors",
                columns: table => new
                {
                    VisitVisitorId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitRequestId = table.Column<long>(type: "bigint", nullable: false),
                    VisitorId = table.Column<long>(type: "bigint", nullable: false),
                    QRTokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QRTokenProtected = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    QRGeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QRGeneratedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedInByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedOutByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitVisitors", x => x.VisitVisitorId);
                    table.ForeignKey(
                        name: "FK_VisitVisitors_AspNetUsers_CheckedInByUserId",
                        column: x => x.CheckedInByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitVisitors_AspNetUsers_CheckedOutByUserId",
                        column: x => x.CheckedOutByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitVisitors_AspNetUsers_QRGeneratedByUserId",
                        column: x => x.QRGeneratedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitVisitors_VisitRequests_VisitRequestId",
                        column: x => x.VisitRequestId,
                        principalTable: "VisitRequests",
                        principalColumn: "VisitRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitVisitors_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "VisitorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_VisitFromDateTime",
                table: "VisitRequests",
                column: "VisitFromDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_VisitToDateTime",
                table: "VisitRequests",
                column: "VisitToDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_IdNumber",
                table: "Visitors",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitVisitors_CheckedInByUserId",
                table: "VisitVisitors",
                column: "CheckedInByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitVisitors_CheckedOutByUserId",
                table: "VisitVisitors",
                column: "CheckedOutByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitVisitors_QRGeneratedByUserId",
                table: "VisitVisitors",
                column: "QRGeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitVisitors_QRTokenHash",
                table: "VisitVisitors",
                column: "QRTokenHash",
                unique: true,
                filter: "[QRTokenHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VisitVisitors_VisitorId",
                table: "VisitVisitors",
                column: "VisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitVisitors_VisitRequestId_VisitorId",
                table: "VisitVisitors",
                columns: new[] { "VisitRequestId", "VisitorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitVisitors");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_VisitFromDateTime",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_VisitToDateTime",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_IdNumber",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "VisitFromDateTime",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "VisitToDateTime",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "IdExpiryDate",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "IdType",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Visitors");

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

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ExpectedArrivalTime",
                table: "VisitRequests",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ExpectedDepartureTime",
                table: "VisitRequests",
                type: "time",
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

            migrationBuilder.AddColumn<string>(
                name: "QRTokenProtected",
                table: "VisitRequests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "VisitDate",
                table: "VisitRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<long>(
                name: "VisitorId",
                table: "VisitRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Visitors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

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

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_Status",
                table: "VisitRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_VisitDate",
                table: "VisitRequests",
                column: "VisitDate");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_VisitorId",
                table: "VisitRequests",
                column: "VisitorId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_Visitors_VisitorId",
                table: "VisitRequests",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "VisitorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
