using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e19719e1-f42a-4fd3-acc5-4ce68392c502", "AQAAAAIAAYagAAAAEJLrQiSzTX0IA5zbPE/Jb7hejTjNODvZ5qz7EOpxW7FynU/FU+/xnzMkbkDEKv+YBA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "46295bfd-0cd5-4e78-8e01-270b0fca53c6", "AQAAAAIAAYagAAAAELi0wRMVmjSgYfmfOXBsLBfZVvxMX+FKZsVlgRBGA25gYjZB0BRhMiWAnoLp6buKxg==" });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 20, 14, 58, 31, 53, DateTimeKind.Utc).AddTicks(6176));

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 20, 14, 58, 31, 53, DateTimeKind.Utc).AddTicks(6183));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 20, 14, 58, 31, 53, DateTimeKind.Utc).AddTicks(6212));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 20, 14, 58, 31, 53, DateTimeKind.Utc).AddTicks(6224));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2aa6b5f3-4ffc-4a7e-9541-09f41bf9dd6e", "AQAAAAIAAYagAAAAEAW5YOCztDZc9B9AYLrI68Q/PbfguPQRvP8htibuHdP+SFZsqoquvdGx3xjIcP2meQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ec63d153-10f4-468a-a30e-49d4bd14d0f4", "AQAAAAIAAYagAAAAEN7IJFK9PlRavph8Y2Xwd+bfdVikpBssPa8MjnAydogIntWdnBL1X53G12uHuPFkDw==" });

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 14, 34, 21, 727, DateTimeKind.Utc).AddTicks(3651));

            migrationBuilder.UpdateData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 14, 34, 21, 727, DateTimeKind.Utc).AddTicks(3656));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 14, 34, 21, 727, DateTimeKind.Utc).AddTicks(3680));

            migrationBuilder.UpdateData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2025, 5, 28, 14, 34, 21, 727, DateTimeKind.Utc).AddTicks(3689));
        }
    }
}
