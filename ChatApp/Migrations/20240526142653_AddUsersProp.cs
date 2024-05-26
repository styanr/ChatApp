using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatRoomUsers_Users_UsersId",
                table: "GroupChatRoomUsers");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "GroupChatRoomUsers",
                newName: "UserListId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupChatRoomUsers_UsersId",
                table: "GroupChatRoomUsers",
                newName: "IX_GroupChatRoomUsers_UserListId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ChatRooms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_UserId",
                table: "ChatRooms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Users_UserId",
                table: "ChatRooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatRoomUsers_Users_UserListId",
                table: "GroupChatRoomUsers",
                column: "UserListId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Users_UserId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatRoomUsers_Users_UserListId",
                table: "GroupChatRoomUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_UserId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChatRooms");

            migrationBuilder.RenameColumn(
                name: "UserListId",
                table: "GroupChatRoomUsers",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupChatRoomUsers_UserListId",
                table: "GroupChatRoomUsers",
                newName: "IX_GroupChatRoomUsers_UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatRoomUsers_Users_UsersId",
                table: "GroupChatRoomUsers",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
