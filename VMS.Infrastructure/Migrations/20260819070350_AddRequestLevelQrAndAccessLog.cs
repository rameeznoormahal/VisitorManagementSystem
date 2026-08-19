using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestLevelQrAndAccessLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_CreatedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_DecisionByUserId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_HostUserId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_Departments_DepartmentId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitVisitors_AspNetUsers_CheckedInByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitVisitors_AspNetUsers_CheckedOutByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitVisitors_AspNetUsers_QRGeneratedByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropIndex(
                name: "IX_VisitVisitors_CheckedInByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropIndex(
                name: "IX_VisitVisitors_CheckedOutByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropIndex(
                name: "IX_VisitVisitors_QRGeneratedByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropIndex(
                name: "IX_VisitVisitors_QRTokenHash",
                table: "VisitVisitors");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_CreatedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_DecisionByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_HostUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_VisitReference",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "CheckedInByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "CheckedOutByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "QRGeneratedByUserId",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "QRGeneratedDate",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "QRTokenHash",
                table: "VisitVisitors");

            migrationBuilder.DropColumn(
                name: "QRTokenProtected",
                table: "VisitVisitors");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VisitVisitors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "VisitReference",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLocation",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HostUserId",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "DecisionComments",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DecisionByUserId",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "VisitRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

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

            migrationBuilder.CreateTable(
                name: "VisitAccessLogs",
                columns: table => new
                {
                    VisitAccessLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitRequestId = table.Column<long>(type: "bigint", nullable: false),
                    VisitVisitorId = table.Column<long>(type: "bigint", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryProcessedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ExitProcessedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EntryGateOrLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExitGateOrLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitAccessLogs", x => x.VisitAccessLogId);
                    table.ForeignKey(
                        name: "FK_VisitAccessLogs_AspNetUsers_EntryProcessedByUserId",
                        column: x => x.EntryProcessedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitAccessLogs_AspNetUsers_ExitProcessedByUserId",
                        column: x => x.ExitProcessedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitAccessLogs_VisitRequests_VisitRequestId",
                        column: x => x.VisitRequestId,
                        principalTable: "VisitRequests",
                        principalColumn: "VisitRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitAccessLogs_VisitVisitors_VisitVisitorId",
                        column: x => x.VisitVisitorId,
                        principalTable: "VisitVisitors",
                        principalColumn: "VisitVisitorId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_VisitAccessLogs_EntryProcessedByUserId",
                table: "VisitAccessLogs",
                column: "EntryProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitAccessLogs_EntryTime",
                table: "VisitAccessLogs",
                column: "EntryTime");

            migrationBuilder.CreateIndex(
                name: "IX_VisitAccessLogs_ExitProcessedByUserId",
                table: "VisitAccessLogs",
                column: "ExitProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitAccessLogs_ExitTime",
                table: "VisitAccessLogs",
                column: "ExitTime");

            migrationBuilder.CreateIndex(
                name: "IX_VisitAccessLogs_VisitRequestId",
                table: "VisitAccessLogs",
                column: "VisitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitAccessLogs_VisitVisitorId_EntryTime",
                table: "VisitAccessLogs",
                columns: new[] { "VisitVisitorId", "EntryTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_QRGeneratedByUserId",
                table: "VisitRequests",
                column: "QRGeneratedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_Departments_DepartmentId",
                table: "VisitRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_AspNetUsers_QRGeneratedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitRequests_Departments_DepartmentId",
                table: "VisitRequests");

            migrationBuilder.DropTable(
                name: "VisitAccessLogs");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_QRGeneratedByUserId",
                table: "VisitRequests");

            migrationBuilder.DropIndex(
                name: "IX_VisitRequests_QRTokenHash",
                table: "VisitRequests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VisitVisitors");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDate",
                table: "VisitVisitors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutDate",
                table: "VisitVisitors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedInByUserId",
                table: "VisitVisitors",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedOutByUserId",
                table: "VisitVisitors",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRGeneratedByUserId",
                table: "VisitVisitors",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QRGeneratedDate",
                table: "VisitVisitors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRTokenHash",
                table: "VisitVisitors",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRTokenProtected",
                table: "VisitVisitors",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VisitReference",
                table: "VisitRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "VisitRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "VisitRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLocation",
                table: "VisitRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HostUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DecisionComments",
                table: "VisitRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DecisionByUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "VisitRequests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                name: "IX_VisitRequests_CreatedByUserId",
                table: "VisitRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_DecisionByUserId",
                table: "VisitRequests",
                column: "DecisionByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_HostUserId",
                table: "VisitRequests",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_VisitReference",
                table: "VisitRequests",
                column: "VisitReference",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_CreatedByUserId",
                table: "VisitRequests",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_DecisionByUserId",
                table: "VisitRequests",
                column: "DecisionByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_AspNetUsers_HostUserId",
                table: "VisitRequests",
                column: "HostUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitRequests_Departments_DepartmentId",
                table: "VisitRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitVisitors_AspNetUsers_CheckedInByUserId",
                table: "VisitVisitors",
                column: "CheckedInByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitVisitors_AspNetUsers_CheckedOutByUserId",
                table: "VisitVisitors",
                column: "CheckedOutByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitVisitors_AspNetUsers_QRGeneratedByUserId",
                table: "VisitVisitors",
                column: "QRGeneratedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
