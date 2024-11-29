using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheCommentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "135b9a82-1cf5-43c8-90ef-b811bf643be2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "da51afa1-06ea-419f-9d3a-5ecc48faa060");

            migrationBuilder.AddColumn<Guid>(
                name: "RootId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "d03dcaee-6367-4c3a-9501-ef53bbbde488", null, "Admin", "ADMIN" },
                    { "eddaecba-86d4-4601-99ed-13123d7b50d3", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d03dcaee-6367-4c3a-9501-ef53bbbde488");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eddaecba-86d4-4601-99ed-13123d7b50d3");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Comments");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "135b9a82-1cf5-43c8-90ef-b811bf643be2", null, "Admin", "ADMIN" },
                    { "da51afa1-06ea-419f-9d3a-5ecc48faa060", null, "User", "USER" }
                });
        }
    }
}
