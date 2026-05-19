using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations
{
    /// <inheritdoc />
    public partial class added_renamings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_Dependency",
                table: "EntityProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_Dependency",
                table: "EntityProxies");

            migrationBuilder.RenameColumn(
                name: "Dependency",
                table: "EntityProxies",
                newName: "EntityDependencyProxy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies",
                column: "EntityDependencyProxy",
                unique: true,
                filter: "[EntityDependencyProxy] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxy",
                table: "EntityProxies",
                column: "EntityDependencyProxy",
                principalTable: "EntityDependencyProxies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxy",
                table: "EntityProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies");

            migrationBuilder.RenameColumn(
                name: "EntityDependencyProxy",
                table: "EntityProxies",
                newName: "Dependency");

            migrationBuilder.CreateIndex(
                name: "IX_EntityProxies_Dependency",
                table: "EntityProxies",
                column: "Dependency",
                unique: true,
                filter: "[Dependency] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_Dependency",
                table: "EntityProxies",
                column: "Dependency",
                principalTable: "EntityDependencyProxies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
