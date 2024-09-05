using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Migrations
{
    public partial class updateIDs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendshipId",
                table: "Chats");

            migrationBuilder.AddColumn<string>(
                name: "FriendId",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendId",
                table: "Chats");

            migrationBuilder.AddColumn<int>(
                name: "FriendshipId",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
