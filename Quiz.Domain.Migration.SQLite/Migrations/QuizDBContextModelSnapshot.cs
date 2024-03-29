﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Quiz.Domain;

#nullable disable

namespace Quiz.Domain.Migrations
{
    [DbContext(typeof(QuizDBContext))]
    partial class QuizDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("Quiz.Domain.Exams.Choice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Statement")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("qzChoices");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Exam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AncestorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasMaxLength(16)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("FullMarks")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT");

                    b.Property<bool>("RandomChoicesAllowed")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AncestorId");

                    b.HasIndex("Code");

                    b.ToTable("qzExams");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasMaxLength(16)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ExamId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUri")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Statement")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.HasIndex("Position");

                    b.ToTable("qzQuestions");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Sessions.CandidateNote", b =>
                {
                    b.Property<string>("CandidateId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ExamId")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMarkedAsDoubt")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMarkedAsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<int>("NumOfWrongAnswers")
                        .HasColumnType("INTEGER");

                    b.HasKey("CandidateId", "QuestionId");

                    b.HasIndex("ExamId");

                    b.HasIndex("QuestionId");

                    b.ToTable("qzCandidateNotes");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Sessions.ExamSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CandidateId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ExamId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExamName")
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ExecutedOn")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("FullMarks")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsEnded")
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("Marks")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("NumOfCorrectAnswers")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumOfQuestions")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumOfWrongAnswers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("QSequence")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.HasIndex("CandidateId", "ExamId");

                    b.ToTable("qzExamSessions");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Sessions.ExamSessionItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAnswered")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMarkedAsDoubt")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMarkedAsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("qzExamSessionItems");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Choice", b =>
                {
                    b.HasOne("Quiz.Domain.Exams.Question", "Question")
                        .WithMany("Choices")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Exam", b =>
                {
                    b.HasOne("Quiz.Domain.Exams.Exam", "Ancestor")
                        .WithMany()
                        .HasForeignKey("AncestorId");

                    b.Navigation("Ancestor");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Question", b =>
                {
                    b.HasOne("Quiz.Domain.Exams.Exam", "Exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Sessions.CandidateNote", b =>
                {
                    b.HasOne("Quiz.Domain.Exams.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Sessions.ExamSession", b =>
                {
                    b.HasOne("Quiz.Domain.Exams.Exam", "Exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Sessions.ExamSessionItem", b =>
                {
                    b.HasOne("Quiz.Domain.Exams.Sessions.ExamSession", "Session")
                        .WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Session");
                });

            modelBuilder.Entity("Quiz.Domain.Exams.Question", b =>
                {
                    b.Navigation("Choices");
                });
#pragma warning restore 612, 618
        }
    }
}
