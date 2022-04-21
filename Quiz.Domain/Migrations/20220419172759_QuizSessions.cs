﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz.Domain.Migrations
{
    public partial class QuizSessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "qzCandidateNotes",
                columns: table => new
                {
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumOfWrongAnswers = table.Column<int>(type: "int", nullable: false),
                    IsMarkedAsDoubt = table.Column<bool>(type: "bit", nullable: false),
                    IsMarkedAsHidden = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzCandidateNotes", x => new { x.CandidateId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_qzCandidateNotes_qzCandidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "qzCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qzCandidateNotes_qzQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "qzQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qzExamSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ExecutedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    QSequence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumOfQuestions = table.Column<int>(type: "int", nullable: false),
                    NumOfCorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    NumOfWrongAnswers = table.Column<int>(type: "int", nullable: false),
                    Marks = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzExamSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qzExamSessions_qzExams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "qzExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qzExamSessionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    IsMarkedAsDoubt = table.Column<bool>(type: "bit", nullable: false),
                    IsMarkedAsHidden = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzExamSessionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qzExamSessionItems_qzExamSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "qzExamSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_qzCandidateNotes_ExamId",
                table: "qzCandidateNotes",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_qzCandidateNotes_QuestionId",
                table: "qzCandidateNotes",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_qzExamSessionItems_SessionId",
                table: "qzExamSessionItems",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_qzExamSessions_CandidateId_ExamId",
                table: "qzExamSessions",
                columns: new[] { "CandidateId", "ExamId" });

            migrationBuilder.CreateIndex(
                name: "IX_qzExamSessions_ExamId",
                table: "qzExamSessions",
                column: "ExamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "qzCandidateNotes");

            migrationBuilder.DropTable(
                name: "qzExamSessionItems");

            migrationBuilder.DropTable(
                name: "qzExamSessions");
        }
    }
}
