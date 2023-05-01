using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class viewtoSearchContracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE  View CurrentContractversions
AS
SELECT     ContractVersion_Authors.Name_LastName as LastName, ContractVersions.WorkingTitle, ContractVersions.ContractId, ContractVersion_Authors.Name_FirstName AS FirstName, Contracts.DateInitiated, Contracts.ContractNumber, Contracts.CurrentVersionId,
                  ContractVersions.Id AS versionid
FROM        ContractVersion_Authors INNER JOIN
                  ContractVersions ON ContractVersion_Authors.ContractVersionId = ContractVersions.Id INNER JOIN
                  Contracts ON ContractVersions.ContractId = Contracts.Id AND ContractVersions.Id = Contracts.CurrentVersionId"
);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP  View CurrentContractversions");
        }
    }
}
