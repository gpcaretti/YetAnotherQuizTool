using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz.Domain.Migrations
{
    public partial class QuizSessionsImprovement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnded",
                table: "qzExamSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnswered",
                table: "qzExamSessionItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1a5442a6-de43-d043-8b50-3a0356472148"),
                column: "Name",
                value: "Manager");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d29bb6f-d47c-ac17-0256-3a035ce14eb3"),
                column: "Name",
                value: "Candidate");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9a8dc12f-c862-e400-6e11-3a0355e4c36f"),
                column: "Name",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("36bace61-5b6a-6bd6-6050-3a030665f9f7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "484a1d78-58de-4e8c-b84c-21430c228535", "AQAAAAEAACcQAAAAEH2FdmizH2/CrqyytU94wVRPnzkfC3Df2gCD7XyiE2NtVU/+tXNIznRXUM3nzZNtVA==" });

            migrationBuilder.CreateIndex(
                name: "IX_qzExams_Code",
                table: "qzExams",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_qzExams_Code",
                table: "qzExams");

            migrationBuilder.DropColumn(
                name: "IsEnded",
                table: "qzExamSessions");

            migrationBuilder.DropColumn(
                name: "IsAnswered",
                table: "qzExamSessionItems");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1a5442a6-de43-d043-8b50-3a0356472148"),
                column: "Name",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d29bb6f-d47c-ac17-0256-3a035ce14eb3"),
                column: "Name",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9a8dc12f-c862-e400-6e11-3a0355e4c36f"),
                column: "Name",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("36bace61-5b6a-6bd6-6050-3a030665f9f7"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "24249d8f-f879-4f87-8fbb-7d398074455e", "AQAAAAEAACcQAAAAEGlLpoPTf1l26BreGuUAuloWLdxdt9qQ5FcQnKAz1PBLtE9czOQzpJhhFVeEd3rGxg==" });
        }
    }
}
