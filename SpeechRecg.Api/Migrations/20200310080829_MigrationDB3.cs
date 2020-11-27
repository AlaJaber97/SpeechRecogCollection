using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpeechRecg.Api.Migrations
{
    public partial class MigrationDB3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "TransText",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "TransData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "TransData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReviewBy",
                table: "TransData",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "AudioFile",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "TransText");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "TransData");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "TransData");

            migrationBuilder.DropColumn(
                name: "ReviewBy",
                table: "TransData");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "AudioFile");
        }
    }
}
