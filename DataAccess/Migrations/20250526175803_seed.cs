using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("b205bcf1-c3a1-4f8b-aa6b-88e81a23f88a"), null, "Reader", "READER" },
                    { new Guid("ed38b2f6-7c33-4762-a885-198306c97120"), null, "Writer", "WRITER" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "36a001c9-4c0f-4834-ba6e-c8674693370e", "AQAAAAIAAYagAAAAENCHAzQAnuYJI7xy7ZMiFDS45Gtuh6ciaJq75KmZh5KcFXs1YQ8slYuQYu9n11kC8g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a1ae2fa9-0941-4e26-8f71-b6e4791119e9", "AQAAAAIAAYagAAAAEDBG09mfKKz9Kfn3/D2inUOdTMa66vl5FX0fP8gt8J/n/MD4wkDg/7Doff/2yQp1/w==" });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 17, 58, 2, 613, DateTimeKind.Utc).AddTicks(5680));

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 17, 58, 2, 613, DateTimeKind.Utc).AddTicks(5688));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 17, 58, 2, 613, DateTimeKind.Utc).AddTicks(5718));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 17, 58, 2, 613, DateTimeKind.Utc).AddTicks(5727));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b205bcf1-c3a1-4f8b-aa6b-88e81a23f88a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ed38b2f6-7c33-4762-a885-198306c97120"));

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "08eb9808-cd8b-4ea1-835c-a6fdd4e060e6", "AQAAAAIAAYagAAAAEDLB9gygDahFhyXKGlJQy/zN0pDGA1o0/TLpnuANj7uPq62ly/rZnZRAhNb49abTlg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "960a54d2-4ef9-4aac-adbe-9c2fdf0d9d1d", "AQAAAAIAAYagAAAAEB/brE7799nwTFF5CWkE1K4bCgVpnZwPxWxh3UmuN3yElnLh/8WcS9gsKVJ7Ml4/RQ==" });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 25, 18, 30, 53, 379, DateTimeKind.Utc).AddTicks(4416));

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 25, 18, 30, 53, 379, DateTimeKind.Utc).AddTicks(4420));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 25, 18, 30, 53, 379, DateTimeKind.Utc).AddTicks(4445));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 25, 18, 30, 53, 379, DateTimeKind.Utc).AddTicks(4452));

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4558c0ce-e241-4974-b634-39d44b3835b8", null, "Reader", "READER" },
                    { "ea00ce47-86fd-4187-bf8b-f13b272c08da", null, "Writer", "WRITER" }
                });
        }
    }
}
