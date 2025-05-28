using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b205bcf1-c3a1-4f8b-aa6b-88e81a23f88a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ed38b2f6-7c33-4762-a885-198306c97120"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("61f3f94a-4883-4047-8ddd-cf6c8aa5e15d"), null, "Writer", "WRITER" },
                    { new Guid("e06a728f-77d7-4852-87e8-d5645758c93a"), null, "Reader", "READER" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "84041444-f235-44f4-831f-3a3ec7dca778", "AQAAAAIAAYagAAAAEI4gNBWPhZentYeumlTswDerb94cZM0B7/vqwBJrihU58N/QUunW5ceEvK2oc/W0dQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b9969ec4-d208-498d-8940-ee76ceda3e3b", "AQAAAAIAAYagAAAAEKeLFKoQF4Y4Uo6LFq0IvQxonh5SNTKhdmkab7yom6oUG+I1IClDMon49V4hXNhhdg==" });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2025, 5, 28, 12, 37, 10, 286, DateTimeKind.Utc).AddTicks(6255), null });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2025, 5, 28, 12, 37, 10, 286, DateTimeKind.Utc).AddTicks(6261), null });

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 12, 37, 10, 286, DateTimeKind.Utc).AddTicks(6292));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 12, 37, 10, 286, DateTimeKind.Utc).AddTicks(6303));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("61f3f94a-4883-4047-8ddd-cf6c8aa5e15d"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e06a728f-77d7-4852-87e8-d5645758c93a"));

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "BlogPosts");

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
    }
}
