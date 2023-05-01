using Microsoft.EntityFrameworkCore;
using PublisherSystem.SharedKernel.DTOs;

namespace Infrastructure.Data.Services;

public class ContractSearchService
{
    SearchContext _context;

    public ContractSearchService(SearchContext context)
    {
        _context = context;
    }

    public async Task<List<GuidKeyAndDescription>> 
        GetContractPickListForAuthorLastName(string lastnameStart)
    {
        return _context.SearchResults.FromSqlInterpolated
            ($"GetContractsForAuthorLastNameStartswith {lastnameStart}").ToList();
    }

    public async Task<List<GuidKeyAndDescription>> 
        GetContractPickListForInitiatedDateRange(DateTime datestart, DateTime dateend)
    {
        return _context.SearchResults.FromSqlInterpolated
            ($"GetContractsInitiatedInDateRange {datestart},{dateend}").ToList();


    }
    //other options 
    //all contracts? Unsigned contracts? Abandoned contracts?
}
