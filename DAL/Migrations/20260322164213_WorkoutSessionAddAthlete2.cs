using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutSessionAddAthlete2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises",
                column: "WorkoutSessionId",
                principalTable: "WorkoutSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises",
                column: "WorkoutSessionId",
                principalTable: "WorkoutSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
