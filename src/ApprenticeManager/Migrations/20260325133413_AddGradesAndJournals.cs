using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApprenticeManager.Migrations
{
    /// <inheritdoc />
    public partial class AddGradesAndJournals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningJournals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApprenticeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Kalenderwoche = table.Column<int>(type: "INTEGER", nullable: false),
                    Jahr = table.Column<int>(type: "INTEGER", nullable: false),
                    BetrieblicheTaetigkeiten = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    SchulischeTaetigkeiten = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Reflexion = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningJournals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningJournals_Apprentices_ApprenticeId",
                        column: x => x.ApprenticeId,
                        principalTable: "Apprentices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ApprenticeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Apprentices_ApprenticeId",
                        column: x => x.ApprenticeId,
                        principalTable: "Apprentices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", precision: 3, scale: 1, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SubjectId",
                table: "Grades",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningJournals_ApprenticeId",
                table: "LearningJournals",
                column: "ApprenticeId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ApprenticeId",
                table: "Subjects",
                column: "ApprenticeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "LearningJournals");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
