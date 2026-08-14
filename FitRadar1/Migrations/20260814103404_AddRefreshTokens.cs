using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRadar.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Providers_Id",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Facilities",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Facilities",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Facilities",
                newName: "Address");

            migrationBuilder.AddColumn<Guid>(
                name: "ProviderId",
                table: "Packages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ProviderId",
                table: "Packages",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Providers_ProviderId",
                table: "Packages",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Providers_ProviderId",
                table: "Packages");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Packages_ProviderId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Facilities",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Facilities",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Facilities",
                newName: "Adress");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Providers_Id",
                table: "Packages",
                column: "Id",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
