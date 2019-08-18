using System;
using Microsoft.EntityFrameworkCore.Migrations;
// ReSharper disable All

namespace IntegrationEventLog.Migrations
{
    public partial class InitialIntegrationEventLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "integration_event_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TimesSent = table.Column<int>(nullable: false),
                    Content = table.Column<string>(type: "json", nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integration_event_logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "integration_event_logs");
        }
    }
}
