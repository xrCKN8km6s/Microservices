using Microsoft.EntityFrameworkCore.Migrations;

namespace Users.API.Migrations
{
    public partial class ExtendedUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "sub",
                keyValue: "affc1ed6-e923-461f-8199-e95c07dc373b",
                columns: new[] { "name", "email" },
                values: new object[] { "Alice Smith", "AliceSmith@email.com" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "sub",
                keyValue: "6ddbe58f-b173-47b5-bc65-848833a93ba2",
                columns: new[] { "name", "email" },
                values: new object[] { "Bob Smith", "BobSmith@email.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "name",
                table: "users");
        }
    }
}
