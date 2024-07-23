using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateAttribuets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "porjectName",
                table: "AspNetUsers",
                newName: "projectName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "projectName",
                table: "AspNetUsers",
                newName: "porjectName");
        }
    }
}
