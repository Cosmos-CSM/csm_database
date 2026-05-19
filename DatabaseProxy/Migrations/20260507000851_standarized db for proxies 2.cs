using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations
{
    /// <inheritdoc />
    public partial class standarizeddbforproxies2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxyId",
                table: "EntityProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_EntityDependencyProxyId",
                table: "EntityProxies");

            migrationBuilder.DropColumn(
                name: "EntityDependencyProxyId",
                table: "EntityProxies");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies",
                column: "EntityDependencyProxy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies");

            migrationBuilder.AddColumn<long>(
                name: "EntityDependencyProxyId",
                table: "EntityProxies",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies",
                column: "EntityDependencyProxy",
                unique: true,
                filter: "[EntityDependencyProxy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProxies_EntityDependencyProxyId",
                table: "EntityProxies",
                column: "EntityDependencyProxyId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxyId",
                table: "EntityProxies",
                column: "EntityDependencyProxyId",
                principalTable: "EntityDependencyProxies",
                principalColumn: "Id");
        }
    }
}
