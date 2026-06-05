using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COVID19.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryRegions",
                columns: table => new
                {
                    CountryRegionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryRegions", x => x.CountryRegionId);
                });

            migrationBuilder.CreateTable(
                name: "ProvinceStates",
                columns: table => new
                {
                    ProvinceStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryRegionId = table.Column<int>(type: "int", nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Long = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvinceStates", x => x.ProvinceStateId);
                    table.ForeignKey(
                        name: "FK_ProvinceStates_CountryRegions_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegions",
                        principalColumn: "CountryRegionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CovidGlobalConfirmeds",
                columns: table => new
                {
                    CovidGlobalConfirmedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryRegionId = table.Column<int>(type: "int", nullable: true),
                    ProvinceStateId = table.Column<int>(type: "int", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedCases = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CovidGlobalConfirmeds", x => x.CovidGlobalConfirmedId);
                    table.ForeignKey(
                        name: "FK_CovidGlobalConfirmeds_CountryRegions_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegions",
                        principalColumn: "CountryRegionId");
                    table.ForeignKey(
                        name: "FK_CovidGlobalConfirmeds_ProvinceStates_ProvinceStateId",
                        column: x => x.ProvinceStateId,
                        principalTable: "ProvinceStates",
                        principalColumn: "ProvinceStateId");
                });

            migrationBuilder.CreateTable(
                name: "CovidGlobalDeaths",
                columns: table => new
                {
                    CovidGlobalDeathsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryRegionId = table.Column<int>(type: "int", nullable: true),
                    ProvinceStateId = table.Column<int>(type: "int", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deaths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CovidGlobalDeaths", x => x.CovidGlobalDeathsId);
                    table.ForeignKey(
                        name: "FK_CovidGlobalDeaths_CountryRegions_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegions",
                        principalColumn: "CountryRegionId");
                    table.ForeignKey(
                        name: "FK_CovidGlobalDeaths_ProvinceStates_ProvinceStateId",
                        column: x => x.ProvinceStateId,
                        principalTable: "ProvinceStates",
                        principalColumn: "ProvinceStateId");
                });

            migrationBuilder.CreateTable(
                name: "CovidGlobalRecovereds",
                columns: table => new
                {
                    CovidGlobalRecoveredId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryRegionId = table.Column<int>(type: "int", nullable: true),
                    ProvinceStateId = table.Column<int>(type: "int", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecoveredCases = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CovidGlobalRecovereds", x => x.CovidGlobalRecoveredId);
                    table.ForeignKey(
                        name: "FK_CovidGlobalRecovereds_CountryRegions_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegions",
                        principalColumn: "CountryRegionId");
                    table.ForeignKey(
                        name: "FK_CovidGlobalRecovereds_ProvinceStates_ProvinceStateId",
                        column: x => x.ProvinceStateId,
                        principalTable: "ProvinceStates",
                        principalColumn: "ProvinceStateId");
                });

            migrationBuilder.CreateTable(
                name: "DailyReport",
                columns: table => new
                {
                    DailyReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryRegionId = table.Column<int>(type: "int", nullable: true),
                    ProvinceStateId = table.Column<int>(type: "int", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UID = table.Column<int>(type: "int", nullable: true),
                    ISO3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FIPS = table.Column<int>(type: "int", nullable: true),
                    Confirmed = table.Column<int>(type: "int", nullable: false),
                    Deaths = table.Column<int>(type: "int", nullable: false),
                    Recovered = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<int>(type: "int", nullable: false),
                    IncidentRate = table.Column<double>(type: "float", nullable: true),
                    PeopleTested = table.Column<long>(type: "bigint", nullable: true),
                    PeopleHospitalized = table.Column<long>(type: "bigint", nullable: true),
                    TestingRate = table.Column<double>(type: "float", nullable: true),
                    HospitalizationRate = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyReport", x => x.DailyReportId);
                    table.ForeignKey(
                        name: "FK_DailyReport_CountryRegions_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegions",
                        principalColumn: "CountryRegionId");
                    table.ForeignKey(
                        name: "FK_DailyReport_ProvinceStates_ProvinceStateId",
                        column: x => x.ProvinceStateId,
                        principalTable: "ProvinceStates",
                        principalColumn: "ProvinceStateId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CovidGlobalConfirmeds_CountryRegionId",
                table: "CovidGlobalConfirmeds",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CovidGlobalConfirmeds_ProvinceStateId",
                table: "CovidGlobalConfirmeds",
                column: "ProvinceStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CovidGlobalDeaths_CountryRegionId",
                table: "CovidGlobalDeaths",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CovidGlobalDeaths_ProvinceStateId",
                table: "CovidGlobalDeaths",
                column: "ProvinceStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CovidGlobalRecovereds_CountryRegionId",
                table: "CovidGlobalRecovereds",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CovidGlobalRecovereds_ProvinceStateId",
                table: "CovidGlobalRecovereds",
                column: "ProvinceStateId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyReport_CountryRegionId",
                table: "DailyReport",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyReport_ProvinceStateId",
                table: "DailyReport",
                column: "ProvinceStateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvinceStates_CountryRegionId",
                table: "ProvinceStates",
                column: "CountryRegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CovidGlobalConfirmeds");

            migrationBuilder.DropTable(
                name: "CovidGlobalDeaths");

            migrationBuilder.DropTable(
                name: "CovidGlobalRecovereds");

            migrationBuilder.DropTable(
                name: "DailyReport");

            migrationBuilder.DropTable(
                name: "ProvinceStates");

            migrationBuilder.DropTable(
                name: "CountryRegions");
        }
    }
}
