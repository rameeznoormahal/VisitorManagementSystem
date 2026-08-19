using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorAndVisitRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    VisitorId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.VisitorId);
                });

            migrationBuilder.CreateTable(
                name: "VisitRequests",
                columns: table => new
                {
                    VisitRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VisitorId = table.Column<long>(type: "bigint", nullable: false),
                    HostUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    VisitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedArrivalTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ExpectedDepartureTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MeetingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitRequests", x => x.VisitRequestId);
                    table.ForeignKey(
                        name: "FK_VisitRequests_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitRequests_AspNetUsers_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitRequests_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "VisitorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_Email",
                table: "Visitors",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_PhoneNumber",
                table: "Visitors",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_CreatedByUserId",
                table: "VisitRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_DepartmentId",
                table: "VisitRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_HostUserId",
                table: "VisitRequests",
                column: "HostUserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_VisitReference",
                table: "VisitRequests",
                column: "VisitReference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitRequests");

            migrationBuilder.DropTable(
                name: "Visitors");
        }
    }
}
