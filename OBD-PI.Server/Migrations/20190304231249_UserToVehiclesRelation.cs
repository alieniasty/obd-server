using Microsoft.EntityFrameworkCore.Migrations;

namespace OBDPI.Server.Migrations
{
    public partial class UserToVehiclesRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Vehicles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_AspNetUsers_UserId",
                table: "Vehicles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_AspNetUsers_UserId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Vehicles");
        }
    }
}
