using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PushCenter.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         name: "Subscriptions",
                                         columns: table => new
                                         {
                                             Id = table.Column<int>(type: "integer", nullable: false)
                                                       .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                             Endpoint = table.Column<string>(type: "text", nullable: true),
                                             Auth = table.Column<string>(type: "text", nullable: true),
                                             P256Dh = table.Column<string>(type: "text", nullable: true),
                                             SubscriptionType = table.Column<int>(type: "integer", nullable: false)
                                         },
                                         constraints: table => { table.PrimaryKey("PK_Subscriptions", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       name: "Subscriptions");
        }
    }
}