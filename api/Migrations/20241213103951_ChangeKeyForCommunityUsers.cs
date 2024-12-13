using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKeyForCommunityUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityUsers",
                table: "CommunityUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "468e3a75-8137-49d7-b4c5-177c470b8f47");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46f363ad-b075-4118-b897-44ac0e4f70b6");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityUsers",
                table: "CommunityUsers",
                columns: new[] { "CommunityId", "UserId", "CommunityRole" });

            migrationBuilder.CreateTable(
                name: "AddressElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<int>(type: "int", nullable: false),
                    ObjectGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    PrevId = table.Column<int>(type: "int", nullable: false),
                    NextId = table.Column<int>(type: "int", nullable: false),
                    IsActual = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressElement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hierarchy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<int>(type: "int", nullable: false),
                    ParentObjId = table.Column<int>(type: "int", nullable: false),
                    PrevId = table.Column<int>(type: "int", nullable: false),
                    NextId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hierarchy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "House",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<int>(type: "int", nullable: false),
                    ObjectGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddNum1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddNum2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseType = table.Column<byte>(type: "tinyint", nullable: true),
                    AddType1 = table.Column<byte>(type: "tinyint", nullable: true),
                    AddType2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrevId = table.Column<int>(type: "int", nullable: false),
                    NextId = table.Column<int>(type: "int", nullable: false),
                    IsActual = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_House", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2611ceae-ae38-4563-a241-aed13e27b425", null, "User", "USER" },
                    { "e9f1cc0b-17de-44d7-b380-46166afeeb2d", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressElement");

            migrationBuilder.DropTable(
                name: "Hierarchy");

            migrationBuilder.DropTable(
                name: "House");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityUsers",
                table: "CommunityUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2611ceae-ae38-4563-a241-aed13e27b425");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e9f1cc0b-17de-44d7-b380-46166afeeb2d");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityUsers",
                table: "CommunityUsers",
                columns: new[] { "CommunityId", "UserId" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "468e3a75-8137-49d7-b4c5-177c470b8f47", null, "Admin", "ADMIN" },
                    { "46f363ad-b075-4118-b897-44ac0e4f70b6", null, "User", "USER" }
                });
        }
    }
}
