using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAppTemplate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteOnUserActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLog_User_PerformedById",
                table: "UserActivityLog");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLog_User_PerformedById",
                table: "UserActivityLog",
                column: "PerformedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLog_User_PerformedById",
                table: "UserActivityLog");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "User",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLog_User_PerformedById",
                table: "UserActivityLog",
                column: "PerformedById",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
