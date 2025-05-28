using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class adminrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("61f3f94a-4883-4047-8ddd-cf6c8aa5e15d"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e06a728f-77d7-4852-87e8-d5645758c93a"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("7d1e5bfe-3048-4b15-b52a-948f43085fff"), null, "Writer", "WRITER" },
                    { new Guid("92db25b8-8738-4730-8e3d-26586a8f02db"), null, "Reader", "READER" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e66987c0-ad85-4164-ac50-d848c6017007", "AQAAAAIAAYagAAAAEKdBckBFuKIm6/jvsgQuKGRRkeUVMKqVIEiGNJJsitU/M6haVAuZqx8scQKnQgsvEA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "9e7ef0a0-f398-4d67-9061-db317a1012e4", "AQAAAAIAAYagAAAAEP7TNPHuCL7hz1JyycI/oHZUkcPk9SjrdxpSAFuA9exwrX1D7itqyRqf1rZ30Swvxw==" });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4170));

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4180));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4212));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4220));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7d1e5bfe-3048-4b15-b52a-948f43085fff"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("92db25b8-8738-4730-8e3d-26586a8f02db"));

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
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 12, 37, 10, 286, DateTimeKind.Utc).AddTicks(6255));

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 12, 37, 10, 286, DateTimeKind.Utc).AddTicks(6261));

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
    }
}
