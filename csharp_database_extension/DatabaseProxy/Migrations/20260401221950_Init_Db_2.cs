using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations
{
    /// <inheritdoc />
    public partial class Init_Db_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityDependencyProxies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDependencyProxies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityProxies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityDependencyProxyId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityProxies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxyId",
                        column: x => x.EntityDependencyProxyId,
                        principalTable: "EntityDependencyProxies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EntityDependantProxies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityProxyId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDependantProxies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityDependantProxies_EntityProxies_EntityProxyId",
                        column: x => x.EntityProxyId,
                        principalTable: "EntityProxies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityDependantProxies_EntityProxyId",
                table: "EntityDependantProxies",
                column: "EntityProxyId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProxies_EntityDependencyProxyId",
                table: "EntityProxies",
                column: "EntityDependencyProxyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityDependantProxies");

            migrationBuilder.DropTable(
                name: "EntityProxies");

            migrationBuilder.DropTable(
                name: "EntityDependencyProxies");
        }
    }
}
