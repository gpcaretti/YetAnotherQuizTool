using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz.Domain.Migrations
{
    public partial class AddRandomChoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RandomChoicesAllowed",
                table: "qzExams",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RandomChoicesAllowed",
                table: "qzExams");
        }
    }
}
