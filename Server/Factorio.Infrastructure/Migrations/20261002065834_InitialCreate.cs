using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Factorio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blueprints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    BlueprintString = table.Column<string>(type: "TEXT", nullable: false),
                    ScreenshotPath = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blueprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blueprints_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InternalName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    StackSize = table.Column<int>(type: "INTEGER", nullable: true),
                    CraftTime = table.Column<double>(type: "REAL", nullable: true),
                    ProductAmount = table.Column<int>(type: "INTEGER", nullable: true),
                    Stats = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ActionDetails = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    AdminComment = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    IngredientItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Items_IngredientItemId",
                        column: x => x.IngredientItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Category", "CraftTime", "Description", "InternalName", "Name", "ProductAmount", "StackSize", "Stats", "UserId" },
                values: new object[,]
                {
                    { 2, "Intermediate products", 3.2000000000000002, null, "copper-plate", "Copper plate", 1, 100, null, null },
                    { 3, "Intermediate products", 0.5, null, "copper-cable", "Copper cable", 2, 200, null, null },
                    { 4, "Intermediate products", 0.5, null, "electronic-circuit", "Electronic circuit", 1, 200, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), "user1@test.com", "Test User", "$2b$11$GIeYTa5Uz6zPLI2AwbkJAOhB6eVEW0P6YEnEV6PgGtO5wTWbeffwW" },
                    { 2, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), "user2@test.com", "Test User 2", "$2b$11$IVm8leFGjvB9x4NoS29XhOU4oKZnD.mXxcMDOyptJncJlGlBpwPKm" }
                });

            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "ActionDetails", "AdminComment", "CreatedAt", "ItemId", "Status", "Type", "UserId" },
                values: new object[] { 1, "Thin wire used in electronic circuits and power poles.", null, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Pending", "Add", 1 });

            migrationBuilder.InsertData(
                table: "Blueprints",
                columns: new[] { "Id", "AuthorId", "BlueprintString", "CreatedAt", "Description", "ScreenshotPath", "Title" },
                values: new object[] { 1, 1, "SAMPLE-BLUEPRINT-STRING", new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Placeholder entry for testing", null, "Sample blueprint" });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Category", "CraftTime", "Description", "InternalName", "Name", "ProductAmount", "StackSize", "Stats", "UserId" },
                values: new object[] { 1, "Intermediate products", 3.2000000000000002, "Basic building material made by smelting iron ore.", "iron-plate", "Iron plate", 1, 100, null, 2 });

            migrationBuilder.InsertData(
                table: "RecipeIngredients",
                columns: new[] { "Id", "Amount", "IngredientItemId", "ItemId" },
                values: new object[,]
                {
                    { 1, 1, 2, 3 },
                    { 3, 3, 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "ActionDetails", "AdminComment", "CreatedAt", "ItemId", "Status", "Type", "UserId" },
                values: new object[] { 2, "Basic building material made by smelting iron ore.", "Looks good", new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Approved", "Edit", 2 });

            migrationBuilder.InsertData(
                table: "RecipeIngredients",
                columns: new[] { "Id", "Amount", "IngredientItemId", "ItemId" },
                values: new object[] { 2, 1, 1, 4 });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ItemId",
                table: "Actions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_UserId",
                table: "Actions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blueprints_AuthorId",
                table: "Blueprints",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_InternalName",
                table: "Items",
                column: "InternalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientItemId",
                table: "RecipeIngredients",
                column: "IngredientItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_ItemId",
                table: "RecipeIngredients",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "Blueprints");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
