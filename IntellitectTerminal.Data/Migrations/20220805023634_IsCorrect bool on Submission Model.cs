using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntellitectTerminal.Data.Migrations
{
    public partial class IsCorrectboolonSubmissionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Submissions");
        }
    }
}
