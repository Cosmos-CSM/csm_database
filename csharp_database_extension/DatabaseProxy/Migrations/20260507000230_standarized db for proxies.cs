using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseProxy.Migrations;

/// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
public partial class standarizeddbforproxies : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "EntityDependencyProxy",
            table: "EntityProxies",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "EntityProxy",
            table: "EntityDependantProxies",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_EntityProxies_EntityDependencyProxy",
            table: "EntityProxies",
            column: "EntityDependencyProxy",
            unique: true,
            filter: "[EntityDependencyProxy] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_EntityDependantProxies_EntityProxy",
            table: "EntityDependantProxies",
            column: "EntityProxy",
            unique: true,
            filter: "[EntityProxy] IS NOT NULL");

        migrationBuilder.AddForeignKey(
            name: "FK_EntityDependantProxies_EntityProxies_EntityProxy",
            table: "EntityDependantProxies",
            column: "EntityProxy",
            principalTable: "EntityProxies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

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
            name: "FK_EntityDependantProxies_EntityProxies_EntityProxy",
            table: "EntityDependantProxies");

        migrationBuilder.DropForeignKey(
            name: "FK_EntityProxies_EntityDependencyProxies_EntityDependencyProxy",
            table: "EntityProxies");

        migrationBuilder.DropIndex(
            name: "IX_EntityProxies_EntityDependencyProxy",
            table: "EntityProxies");

        migrationBuilder.DropIndex(
            name: "IX_EntityDependantProxies_EntityProxy",
            table: "EntityDependantProxies");

        migrationBuilder.DropColumn(
            name: "EntityDependencyProxy",
            table: "EntityProxies");

        migrationBuilder.DropColumn(
            name: "EntityProxy",
            table: "EntityDependantProxies");
    }
}
