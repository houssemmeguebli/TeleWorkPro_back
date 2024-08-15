using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditManagersMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Requests",
                newName: "ProjectManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_userId",
                table: "Requests",
                newName: "IX_Requests_ProjectManagerId");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "Requests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EmployeeId",
                table: "Requests",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_EmployeeId",
                table: "Requests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_ProjectManagerId",
                table: "Requests",
                column: "ProjectManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_EmployeeId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_ProjectManagerId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_EmployeeId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "ProjectManagerId",
                table: "Requests",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_ProjectManagerId",
                table: "Requests",
                newName: "IX_Requests_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_userId",
                table: "Requests",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
