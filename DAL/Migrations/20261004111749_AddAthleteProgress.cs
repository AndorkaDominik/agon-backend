using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayStreak",
                table: "Athletes");

            migrationBuilder.AddColumn<int>(
                name: "AthleteProgressId",
                table: "Athletes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AthleteProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteId = table.Column<int>(type: "int", nullable: false),
                    StrengthXp = table.Column<int>(type: "int", nullable: false),
                    EnduranceXp = table.Column<int>(type: "int", nullable: false),
                    BodyControlXp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteProgresses_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteProgresses_AthleteId",
                table: "AthleteProgresses",
                column: "AthleteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteProgresses");

            migrationBuilder.DropColumn(
                name: "AthleteProgressId",
                table: "Athletes");

            migrationBuilder.AddColumn<int>(
                name: "DayStreak",
                table: "Athletes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
