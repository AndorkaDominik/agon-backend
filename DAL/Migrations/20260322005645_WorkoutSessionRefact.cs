using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutSessionRefact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "WorkoutSessions",
                newName: "Notes");

            migrationBuilder.AlterColumn<int>(
                name: "AthleteId",
                table: "WorkoutSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WorkoutSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WorkoutSessions");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "WorkoutSessions",
                newName: "Note");

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
                name: "FK_WorkoutSessions_Athletes_AthleteId",
                table: "WorkoutSessions",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
