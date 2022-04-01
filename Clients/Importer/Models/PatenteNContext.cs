using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PatenteN.Importer.Models {
    public partial class PatenteNContext : DbContext {
        public PatenteNContext() {
        }

        public PatenteNContext(DbContextOptions<PatenteNContext> options)
            : base(options) {
        }

        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Sorgente> Sorgentes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=PatenteN;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Answer>(entity => {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Statement).HasMaxLength(4000);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Answers_Questions");
            });

            modelBuilder.Entity<Category>(entity => {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(16);

                entity.Property(e => e.Name).HasMaxLength(1024);

                entity.HasOne(d => d.Ancestor)
                    .WithMany(p => p.InverseAncestor)
                    .HasForeignKey(d => d.AncestorId)
                    .HasConstraintName("FK_SubCategory_Category");
            });

            modelBuilder.Entity<Question>(entity => {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(16);

                entity.Property(e => e.Comment).HasColumnType("text");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.ImageComment).HasColumnType("text");

                entity.Property(e => e.Statement).HasMaxLength(4000);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Category");
            });

            modelBuilder.Entity<Sorgente>(entity => {
                entity.HasNoKey();

                entity.ToTable("Sorgente");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsCorrect)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Position)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Statement).HasColumnType("ntext");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
