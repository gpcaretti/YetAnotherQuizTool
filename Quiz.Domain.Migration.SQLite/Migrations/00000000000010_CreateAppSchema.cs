using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz.Domain.Migrations
{
    public partial class CreateAppSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "qzExams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AncestorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    RandomChoicesAllowed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qzExams_qzExams_AncestorId",
                        column: x => x.AncestorId,
                        principalTable: "qzExams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "qzExamSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidateId = table.Column<string>(type: "TEXT", nullable: true),
                    ExamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsEnded = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExamName = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    ExecutedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    QSequence = table.Column<string>(type: "TEXT", nullable: true),
                    NumOfQuestions = table.Column<int>(type: "INTEGER", nullable: false),
                    NumOfCorrectAnswers = table.Column<int>(type: "INTEGER", nullable: false),
                    NumOfWrongAnswers = table.Column<int>(type: "INTEGER", nullable: false),
                    Marks = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FullMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: true)
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
                name: "qzQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: true),
                    Statement = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUri = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qzQuestions_qzExams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "qzExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qzExamSessionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAnswered = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMarkedAsDoubt = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMarkedAsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "qzCandidateNotes",
                columns: table => new
                {
                    CandidateId = table.Column<string>(type: "TEXT", nullable: false),
                    QuestionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NumOfWrongAnswers = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMarkedAsDoubt = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMarkedAsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzCandidateNotes", x => new { x.CandidateId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_qzCandidateNotes_qzQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "qzQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qzChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Statement = table.Column<string>(type: "TEXT", nullable: true),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qzChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qzChoices_qzQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "qzQuestions",
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
                name: "IX_qzChoices_QuestionId",
                table: "qzChoices",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_qzExams_AncestorId",
                table: "qzExams",
                column: "AncestorId");

            migrationBuilder.CreateIndex(
                name: "IX_qzExams_Code",
                table: "qzExams",
                column: "Code");

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

            migrationBuilder.CreateIndex(
                name: "IX_qzQuestions_ExamId",
                table: "qzQuestions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_qzQuestions_Position",
                table: "qzQuestions",
                column: "Position");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "qzCandidateNotes");

            migrationBuilder.DropTable(
                name: "qzChoices");

            migrationBuilder.DropTable(
                name: "qzExamSessionItems");

            migrationBuilder.DropTable(
                name: "qzQuestions");

            migrationBuilder.DropTable(
                name: "qzExamSessions");

            migrationBuilder.DropTable(
                name: "qzExams");
        }
    }
}
