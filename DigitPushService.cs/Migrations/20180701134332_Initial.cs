using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitPushService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PushChannelConfigurations",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    EndpointInfo = table.Column<string>(nullable: true),
                    Endpoint = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ExpirationTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushChannelConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PushChannelOptions",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    PushChannelConfigurationID = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    EndpointOption = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushChannelOptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PushChannelOptions_PushChannelConfigurations_PushChannelConfigurationID",
                        column: x => x.PushChannelConfigurationID,
                        principalTable: "PushChannelConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PushChannelOptions_PushChannelConfigurationID",
                table: "PushChannelOptions",
                column: "PushChannelConfigurationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushChannelOptions");

            migrationBuilder.DropTable(
                name: "PushChannelConfigurations");
        }
    }
}
