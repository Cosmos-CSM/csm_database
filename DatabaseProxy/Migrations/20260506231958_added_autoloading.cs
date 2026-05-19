using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations
{
    /// <inheritdoc />
    public partial class added_autoloading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Dependency",
                table: "EntityProxies",
                type: "bigint",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_Dependency",
                table: "EntityProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_Dependency",
                table: "EntityProxies");

            migrationBuilder.DropColumn(
                name: "Dependency",
                table: "EntityProxies");
        }
    }
}
