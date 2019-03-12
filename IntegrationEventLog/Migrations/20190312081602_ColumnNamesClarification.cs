using Microsoft.EntityFrameworkCore.Migrations;

namespace IntegrationEventLog.Migrations
{
    public partial class ColumnNamesClarification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "integration_event_logs",
                newName: "EventName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "integration_event_logs",
                newName: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventName",
                table: "integration_event_logs",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "integration_event_logs",
                newName: "Id");
        }
    }
}
