using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class sproctosearchcontractinitdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE PROCEDURE GetContractsInitiatedInDateRange
    @initdatestart date,
    @initdateend date
AS

  select groupednames.contractId as KeyValue,[description]
FROM
    (select contractid,currentversionid, dbo.BuildContractHighlights(dateinitiated,workingtitle,string_agg(FirstName + ' ' + LastName,',')) as description
    from CurrentContractversions
    where currentversionid in 
        (select currentversionid
         from CurrentContractversions
         where cast(dateinitiated as date)>=@initdatestart) and cast(dateinitiated as date)<=@initdateend  
    group by [CurrentVersionId],WorkingTitle,contractid,DateInitiated ) groupednames
       ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
