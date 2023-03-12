using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vue_netcore_chatroom.Migrations
{
    public partial class AddMessageSeenByChatUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageSeenByChatUsers",
                columns: table => new
                {
                    ChatUserId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSeenByChatUsers", x => new { x.ChatUserId, x.MessageId });
                    table.ForeignKey(
                        name: "FK_MessageSeenByChatUsers_ChatUsers_ChatUserId",
                        column: x => x.ChatUserId,
                        principalTable: "ChatUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageSeenByChatUsers_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageSeenByChatUsers_MessageId",
                table: "MessageSeenByChatUsers",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageSeenByChatUsers");
        }
    }
}
