using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IntegrationEventLog.Migrations
{
    public partial class IncludeTransactionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "integration_event_logs",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "integration_event_logs");
        }
    }
}
