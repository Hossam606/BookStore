using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShoppingCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_unitprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "CartDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "CartDetails");
        }
    }
}
