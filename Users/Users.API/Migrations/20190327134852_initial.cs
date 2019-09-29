using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Users.API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: false),
                    is_global = table.Column<bool>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    sub = table.Column<string>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permission_roles",
                columns: table => new
                {
                    permission = table.Column<long>(nullable: false),
                    role_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_roles", x => new { x.role_id, x.permission });
                    table.ForeignKey(
                        name: "FK_permission_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    role_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_permission_roles_role_id_permission",
                table: "permission_roles",
                columns: new[] { "role_id", "permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_id",
                table: "roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id_user_id",
                table: "user_roles",
                columns: new[] { "role_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_id",
                table: "users",
                column: "id");

            migrationBuilder.InsertData(
                "users",
                new[] {"sub", "is_active"},
                new object[,]
                {
                    {"affc1ed6-e923-461f-8199-e95c07dc373b", true},
                    {"6ddbe58f-b173-47b5-bc65-848833a93ba2", true}
                });

            migrationBuilder.InsertData(
                "roles",
                new[] { "name", "is_global", "is_active" },
                new object[,]
                {
                    {"Admin", true, true},
                    {"OrdersManager", false, true}
                });

            migrationBuilder.InsertData(
                "user_roles",
                new[] { "role_id", "user_id" },
                new object[,]
                {
                    {1, 1},
                    {2, 2}
                });

            migrationBuilder.InsertData(
                "permission_roles",
                new[] { "permission", "role_id" },
                new object[,]
                {
                    {1, 2},
                    {2, 2}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permission_roles");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
