using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GScore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupAIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_exam_scores_group_a",
                table: "exam_scores",
                columns: new[] { "subject", "student_id", "score" },
                filter: "deleted_at IS NULL AND score IS NOT NULL AND subject IN ('TOAN', 'VATLI', 'HOAHOC')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_exam_scores_group_a",
                table: "exam_scores");
        }
    }
}
