using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PetWorld.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChatHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: false),
                    Answer = table.Column<string>(type: "TEXT", nullable: false),
                    IterationCount = table.Column<int>(type: "int", nullable: false),
                    RecommendedProducts = table.Column<string>(type: "TEXT", nullable: true),
                    IterationsJson = table.Column<string>(type: "LONGTEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistories", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PetType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Brand = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    InStock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistories");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
