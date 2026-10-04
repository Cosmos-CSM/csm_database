using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations
{
    /// <inheritdoc />
    public partial class standarizeddbforproxies3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityDependantProxies_EntityProxies_EntityProxyId",
                table: "EntityDependantProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityDependantProxies_EntityProxy",
                table: "EntityDependantProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityDependantProxies_EntityProxyId",
                table: "EntityDependantProxies");

            migrationBuilder.DropColumn(
                name: "EntityProxyId",
                table: "EntityDependantProxies");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDependantProxies_EntityProxy",
                table: "EntityDependantProxies",
                column: "EntityProxy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EntityDependantProxies_EntityProxy",
                table: "EntityDependantProxies");

            migrationBuilder.AddColumn<long>(
                name: "EntityProxyId",
                table: "EntityDependantProxies",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityDependantProxies_EntityProxy",
                table: "EntityDependantProxies",
                column: "EntityProxy",
                unique: true,
                filter: "[EntityProxy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EntityDependantProxies_EntityProxyId",
                table: "EntityDependantProxies",
                column: "EntityProxyId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityDependantProxies_EntityProxies_EntityProxyId",
                table: "EntityDependantProxies",
                column: "EntityProxyId",
                principalTable: "EntityProxies",
                principalColumn: "Id");
        }
    }
}
