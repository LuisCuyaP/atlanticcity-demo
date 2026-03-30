using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace notification.backend.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialNotificationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PROCESSED_MESSAGES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MESSAGE_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EVENT_TYPE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    PROCESSED_AT = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROCESSED_MESSAGES", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "UX_PROCESSED_MESSAGES_MSG_TYPE",
                table: "PROCESSED_MESSAGES",
                columns: new[] { "MESSAGE_ID", "EVENT_TYPE" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PROCESSED_MESSAGES");
        }
    }
}
