using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShop.Migrations
{
    /// <inheritdoc />
    public partial class yeni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalisanKazanclari",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToplamKazanc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalisanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalisanKazanclari", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CalisanKazanclari_Calisanlar_CalisanID",
                        column: x => x.CalisanID,
                        principalTable: "Calisanlar",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalisanKazanclari_CalisanID",
                table: "CalisanKazanclari",
                column: "CalisanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalisanKazanclari");
        }
    }
}
