using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Services;

public class ContractServices
{
    ContractContext _context;
    public ContractServices(ContractContext context)
    { _context = context; }

    public void Add(Contract newcontract)
    {
        _context.Contracts.Add(newcontract);
        _context.SaveChanges();
    }

    public void AddRevision(Contract contract)
    {
        //set contract modified without touching versions
        _context.Entry(contract).State = EntityState.Modified;
        //add currentversion will ensure its value objects get added
        //because of EF Core handling, setting state won't
        //include the specs or authors!
        _context.Add(contract.CurrentVersion());
        _context.SaveChanges();
    }

    public void Update(Contract contract)
    {
        _context.Entry(contract).State = EntityState.Modified;
        if (contract.Versions.Count() > 0 
            && contract.CurrentVersion().Id == contract.CurrentVersionId)
        {
            _context.Entry(contract.CurrentVersion()).State = EntityState.Modified;
        }
        _context.SaveChanges();
    }

    public void AcceptCurrentVersion(Guid contractId)
    {
        _context.Database.ExecuteSqlInterpolated(
        @$"UPDATE contractversions SET accepted=1
             WHERE id IN (select currentversionid from contracts where id={contractId})");
    }

    //Needs to be handled via an event and savechanges
    //public void FinalizeContract(Guid contractId, DateTime completed)
    //{
    //    _context.Database.ExecuteSqlInterpolated(
    //    @$"UPDATE contracts SET finalversionid=currentversionid,
    //           completed=1,completeddate= {completed}
    //           WHERE id={contractId}");
    //}

    public void FullfillContract(Guid contractId, DateTime fulfilled)
    {
        _context.Database.ExecuteSqlInterpolated(
        @$"UPDATE contracts SET FullfilledDate= {fulfilled}
               WHERE id={contractId}");
    }

    public Contract GetContractAggregateById(Guid contractId)
    {
        return _context.Contracts.Include(c => c.Versions)
            .FirstOrDefault(c => c.Id == contractId);
    }

    public Contract GetContractWithCurrentVersionOnly(Guid contractId)
    {
        var cvid = _context.Contracts
            .Where(c => c.Id == contractId)
            .Select(c => c.CurrentVersionId)  
            .FirstOrDefault();
        return _context.Contracts
            .Include(c => c.Versions.Where(v => v.Id == cvid))
            .FirstOrDefault(c => c.Id == contractId);
    }

    //get authors for a particular contract
    public List<Author> GetAuthorsFromAllVersionsOfAContract(Guid contractId)
    {
        var authors = _context.Contracts.AsNoTracking().Where(c => c.Id == contractId)
              .SelectMany(c => c.Versions.SelectMany(v => v.Authors))
              .ToList();
        return authors;
    }

}


