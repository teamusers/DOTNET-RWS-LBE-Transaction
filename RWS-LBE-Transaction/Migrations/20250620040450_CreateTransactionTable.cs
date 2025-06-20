using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RWS_LBE_Transaction.Migrations
{
    /// <inheritdoc />
    public partial class CreateTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transaction_id_records",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    transaction_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    transaction_number = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_id_records", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transaction_id_records_transaction_id",
                table: "transaction_id_records",
                column: "transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_id_records_transaction_number",
                table: "transaction_id_records",
                column: "transaction_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction_id_records");
        }
    }
} 