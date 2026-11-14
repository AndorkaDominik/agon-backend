using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseTypesToSessionsAndPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalWeight",
                table: "WorkoutSessions",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "TotalDistance",
                table: "WorkoutSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalReps",
                table: "WorkoutSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "SessionExercises",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Reps",
                table: "SessionExercises",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "SessionExercises",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "SessionExercises",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseType",
                table: "SessionExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "TargetWeight",
                table: "PlanExercises",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "TargetReps",
                table: "PlanExercises",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ExerciseType",
                table: "PlanExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TargetDistance",
                table: "PlanExercises",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetDuration",
                table: "PlanExercises",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDistance",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "TotalReps",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseType",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseType",
                table: "PlanExercises");

            migrationBuilder.DropColumn(
                name: "TargetDistance",
                table: "PlanExercises");

            migrationBuilder.DropColumn(
                name: "TargetDuration",
                table: "PlanExercises");

            migrationBuilder.AlterColumn<double>(
                name: "TotalWeight",
                table: "WorkoutSessions",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "SessionExercises",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Reps",
                table: "SessionExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TargetWeight",
                table: "PlanExercises",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TargetReps",
                table: "PlanExercises",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
