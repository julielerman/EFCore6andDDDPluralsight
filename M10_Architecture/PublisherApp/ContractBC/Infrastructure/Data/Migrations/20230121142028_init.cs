using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateInitiated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinalVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fullfilled = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Specs_AdvanceAmountUSD = table.Column<int>(type: "int", nullable: false),
                    Specs_HardCoverRoyaltyPct = table.Column<int>(type: "int", nullable: false),
                    Specs_SoftCoverRoyaltyPct = table.Column<int>(type: "int", nullable: false),
                    Specs_DigitalRoyaltyPct = table.Column<int>(type: "int", nullable: false),
                    Specs_TranslationRoyaltyUSD = table.Column<int>(type: "int", nullable: false),
                    Specs_PublicityProvided = table.Column<bool>(type: "bit", nullable: false),
                    Specs_AuthorAvailableForPR = table.Column<bool>(type: "bit", nullable: false),
                    Specs_PromoCopiesForAuthor = table.Column<int>(type: "int", nullable: false),
                    Specs_PriceForAddlAuthorCopiesUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkingTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateSentToAuthors = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptanceDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModificationReason = table.Column<int>(type: "int", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false),
                    _hasRevisedSpecSet = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractVersions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractVersion_Authors",
                columns: table => new
                {
                    ContractVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Signed = table.Column<bool>(type: "bit", nullable: false),
                    SignedAuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractVersion_Authors", x => new { x.ContractVersionId, x.Id });
                    table.ForeignKey(
                        name: "FK_ContractVersion_Authors_ContractVersions_ContractVersionId",
                        column: x => x.ContractVersionId,
                        principalTable: "ContractVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersions_ContractId",
                table: "ContractVersions",
                column: "ContractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractVersion_Authors");

            migrationBuilder.DropTable(
                name: "ContractVersions");

            migrationBuilder.DropTable(
                name: "Contracts");
        }
    }
}
