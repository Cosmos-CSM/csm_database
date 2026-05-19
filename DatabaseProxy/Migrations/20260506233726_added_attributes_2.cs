using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations
{
    /// <inheritdoc />
    public partial class added_attributes_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxy",
                table: "EntityProxies");

            migrationBuilder.DropIndex(
                name: "IX_EntityProxies_EntityDependencyProxy",
                table: "EntityProxies");

            migrationBuilder.DropColumn(
                name: "EntityDependencyProxy",
                table: "EntityProxies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EntityDependencyProxy",
                table: "EntityProxies",
                type: "bigint",
                nullable: true);

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
    }
}
