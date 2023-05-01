using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class functionToBuildContractHighlights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF OBJECT_ID (N'BuildContractHighlights') IS NOT NULL
   DROP FUNCTION BuildContractHighlights
GO

CREATE FUNCTION BuildContractHighlights
(	
	@dateinitiated datetime, 
	@workingtitle char(100),
	@authorlist char(100))
RETURNS nvarchar(250)
AS
-- place the body of the function here
BEGIN
       RETURN( 'Contract start:'+   FORMAT( @dateinitiated, 'd', 'en-US' ) +',""'+@workingtitle + '"", Author(s): '+ @authorlist)

END

GO
");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION BuildContractHighlights");

        }
    }
}
