using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Hubtel.Wallets.Api.Migrations
{
    public partial class AddPhonenumberAndScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_OwnerId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_OwnerId",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Wallets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OwnerPhoneNumber",
                table: "Wallets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scheme",
                table: "Wallets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Users",
                nullable: false,
                defaultValue: new byte[] {  });

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Users",
                nullable: false,
                defaultValue: new byte[] {  });

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_OwnerPhoneNumber",
                table: "Wallets",
                column: "OwnerPhoneNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_OwnerPhoneNumber",
                table: "Wallets",
                column: "OwnerPhoneNumber",
                principalTable: "Users",
                principalColumn: "PhoneNumber",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_OwnerPhoneNumber",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_OwnerPhoneNumber",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "OwnerPhoneNumber",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Scheme",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Wallets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_OwnerId",
                table: "Wallets",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_OwnerId",
                table: "Wallets",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
