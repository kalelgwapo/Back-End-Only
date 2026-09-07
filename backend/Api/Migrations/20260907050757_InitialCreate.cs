using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Lookup");

            migrationBuilder.CreateTable(
                name: "LeaveType",
                schema: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxDaysAllowed = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReturnDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedDays = table.Column<int>(type: "int", nullable: false),
                    GeneralComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApplications_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalSchema: "Lookup",
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApplications_Users_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApplications_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "LeaveType",
                columns: new[] { "Id", "IsActive", "MaxDaysAllowed", "Name" },
                values: new object[,]
                {
                    { 1, true, 20, "annual" },
                    { 2, true, 7, "sick" },
                    { 3, true, 3, "emergency" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "jane.doe@example.com", "Jane Doe", true, "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", "Employee", "jdoe" },
                    { 2, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "john.smith@example.com", "John Smith", true, "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", "Employee", "jsmith" },
                    { 3, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "maria.santos@example.com", "Maria Santos", true, "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", "Employee", "msantos" },
                    { 4, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "robert.hale@example.com", "Robert Hale", true, "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", "Manager", "rhale" },
                    { 5, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "priya.nair@example.com", "Priya Nair", true, "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", "Manager", "pnair" },
                    { 6, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "david.kim@example.com", "David Kim", true, "AQAAAAIAAYagAAAAEKmyBZSOdiRCMd5DIuHtL0bfJ4gHH+CaF1tfDxKSrGczzKlfmYrk66lRfODdGYRQAQ==", "Manager", "dkim" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_ApplicantId_Status",
                table: "LeaveApplications",
                columns: new[] { "ApplicantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_LeaveTypeId",
                table: "LeaveApplications",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_ManagerId",
                table: "LeaveApplications",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveType_Name",
                schema: "Lookup",
                table: "LeaveType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveApplications");

            migrationBuilder.DropTable(
                name: "LeaveType",
                schema: "Lookup");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
