using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutSessionAddAthlete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions");

            migrationBuilder.AlterColumn<int>(
                name: "AthleteId",
                table: "WorkoutSessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises",
                column: "WorkoutSessionId",
                principalTable: "WorkoutSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions");

            migrationBuilder.AlterColumn<int>(
                name: "AthleteId",
                table: "WorkoutSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionExercises_WorkoutSessions_WorkoutSessionId",
                table: "SessionExercises",
                column: "WorkoutSessionId",
                principalTable: "WorkoutSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id");
        }
    }
}
