using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GScore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectScoreIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_exam_scores_subject_score",
                table: "exam_scores",
                columns: new[] { "subject", "score" },
                filter: "deleted_at IS NULL AND score IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_exam_scores_subject_score",
                table: "exam_scores");
        }
    }
}
