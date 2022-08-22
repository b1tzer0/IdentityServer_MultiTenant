using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace idpMultiTenant1.Migrations
{
    public partial class Fidor2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FidoStoredCredentials",
                table: "FidoStoredCredentials");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "FidoStoredCredentials",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "FidoStoredCredentials",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FidoStoredCredentials",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FidoStoredCredentials",
                table: "FidoStoredCredentials",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FidoStoredCredentials",
                table: "FidoStoredCredentials");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FidoStoredCredentials");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "FidoStoredCredentials",
                newName: "Username");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "FidoStoredCredentials",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FidoStoredCredentials",
                table: "FidoStoredCredentials",
                column: "Username");
        }
    }
}
