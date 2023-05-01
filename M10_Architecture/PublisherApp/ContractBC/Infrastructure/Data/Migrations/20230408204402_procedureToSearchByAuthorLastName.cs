using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class procedureToSearchByAuthorLastName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
                @"CREATE PROCEDURE GetContractsForAuthorLastNameStartswith
    @LastName varchar(15)
AS
select groupednames.contractId as KeyValue,[description]
FROM
    (select contractid,currentversionid, dbo.BuildContractHighlights( dateinitiated,workingtitle,string_agg(FirstName + ' ' + LastName,',')) as [description] 
    from CurrentContractversions
    where currentversionid in 
        (select currentversionid
         from currentcontractversions
         where left(LastName,len(trim(@LastName)))=trim(@LastName))
    group by CurrentVersionId,WorkingTitle,contractid,DateInitiated)  groupednames
          ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE GetContractsForAuthorLastNameStartswith");

        }
    }
}
