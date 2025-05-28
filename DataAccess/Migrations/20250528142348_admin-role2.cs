using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class adminrole2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7d1e5bfe-3048-4b15-b52a-948f43085fff"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("92db25b8-8738-4730-8e3d-26586a8f02db"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, "Reader", "READER" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, "Writer", "WRITER" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2cf9f6dd-6bf8-467f-9dac-80acddedc1f0", "AQAAAAIAAYagAAAAEKzdQPTxLkwwwHnMUz/NlnOmCEUIjSUAh68YrB1/l3Ye5EZwuJkC26RibggpAVZjKQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "74b67e0b-0526-4cdd-a847-d7fef7b0464d", "AQAAAAIAAYagAAAAEFFODaXVxVsWK+mUM0cBeEBEEOzz429z65RLDfyB9gP6u/dbu+dq6wRMjLTAowdE9A==" });

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "ApplicationUserId", "Content", "CreatedAt", "ImageUrl", "Title" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001"), "This is the content of the first blog post.", new DateTime(2025, 5, 28, 14, 23, 47, 956, DateTimeKind.Utc).AddTicks(4283), null, "First Blog Post" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("00000000-0000-0000-0000-000000000001"), "This is the content of the second blog post.", new DateTime(2025, 5, 28, 14, 23, 47, 956, DateTimeKind.Utc).AddTicks(4292), null, "Second Blog Post" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "ApplicationUserId", "BlogPostId", "Content", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("44444444-4444-4444-4444-444444444444"), "This is a comment on the first blog post.", new DateTime(2025, 5, 28, 14, 23, 47, 956, DateTimeKind.Utc).AddTicks(4323) },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("55555555-5555-5555-5555-555555555555"), "This is a comment on the second blog post.", new DateTime(2025, 5, 28, 14, 23, 47, 956, DateTimeKind.Utc).AddTicks(4334) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

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

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "ApplicationUserId", "Content", "CreatedAt", "ImageUrl", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001"), "This is the content of the first blog post.", new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4170), null, "First Blog Post" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001"), "This is the content of the second blog post.", new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4180), null, "Second Blog Post" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "ApplicationUserId", "BlogPostId", "Content", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("11111111-1111-1111-1111-111111111111"), "This is a comment on the first blog post.", new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4212) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222"), "This is a comment on the second blog post.", new DateTime(2025, 5, 28, 13, 48, 34, 850, DateTimeKind.Utc).AddTicks(4220) }
                });
        }
    }
}
