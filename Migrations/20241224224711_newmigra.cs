using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShop.Migrations
{
    /// <inheritdoc />
    public partial class newmigra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Calisanlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CalismaBaslangici",
                table: "Calisanlar",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 9, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CalismaBitisi",
                table: "Calisanlar",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 17, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalismaBaslangici",
                table: "Calisanlar");

            migrationBuilder.DropColumn(
                name: "CalismaBitisi",
                table: "Calisanlar");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Calisanlar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
