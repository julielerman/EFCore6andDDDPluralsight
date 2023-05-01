using ContractBC.ContractAggregate;
using ContractBC.Enums;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IntegrationTests;

[TestClass]
public class ContractRevisionTests
{
    Contract _contract = new Contract(
       DateTime.Today,
       new List<Author> { Author.UnsignedAuthor("first", "last", "email", "phone") }, "booktitle"
       );
    ContractContext _context;

    public ContractRevisionTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
        optionsBuilder.UseSqlServer(
             "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubContractIntegrationTests");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        var _options = optionsBuilder.Options;
        _context = new ContractContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private JsonSerializerOptions CustomJsonOptions()
    {  
        var options = new JsonSerializerOptions() { WriteIndented = true };
        options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
        options.Converters.Add(new CustomDecimalConverter("F"));
        return options;
    }

    [TestMethod]
    public void PersistedContractWithRevisonReturnsTwoVersions()
    {
        CreateNewContractAndAddRevision();
        //added in for debugging a hard problem
        _context.ChangeTracker.DetectChanges();
        //
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions)
            .FirstOrDefault();
        Assert.AreEqual(2, contractFromDB.Versions.Count());
    }
    [TestMethod]
    public void CanUseFilteredInclude()
    {
        CreateNewContractAndAddRevision();
        var cid = _contract.CurrentVersionId;
        //added in for debugging a hard problem
        _context.ChangeTracker.DetectChanges();
        //
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions.Where(v=>v.Id==cid))
            .FirstOrDefault();
        Assert.AreEqual(1, contractFromDB.Versions.Count());
    }



    [TestMethod]
    public void RevisonUsingSameSpecsStoresHasRevisedAsFalse()
    {
         CreateNewContractAndAddRevision();
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions)
            .FirstOrDefault();
        var value = _context.Set<ContractVersion>().OrderBy(v => v.DateCreated)
            .Select(v => EF.Property<bool>(v, "_hasRevisedSpecSet"))
            .LastOrDefault();
        //EF Core 7 variation:
        //var value = _context.Database.SqlQuery<bool>
        //  ($"SELECT TOP 1 [_hasRevisedSpecSet] as Value FROM ContractVersions Order by DateCreated Desc")
        //  .FirstOrDefault();
        Assert.AreEqual(false, value);
    }
    [TestMethod]
    public void RevisonUsingNewSpecsStoresHasRevisedAsTrue()
    {
        CreateNewContractAndAddRevisionWithNewSpecs();
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions)
            .FirstOrDefault();
        var value = _context.Set<ContractVersion>().OrderBy(v => v.DateCreated)
             .Select(v => EF.Property<bool>(v, "_hasRevisedSpecSet")).LastOrDefault();
        //EF Core 7 variation:
        //var value = _context.Database.SqlQuery<bool>
        //($"SELECT TOP 1 [_hasRevisedSpecSet] as Value FROM ContractVersions Order by DateCreated Desc")
        //.FirstOrDefault();
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void PersistedContractWithRevisonAndSameAuthorsDuplicatesAuthorInDatabase()
    {
        var authorsv1 = _contract.CurrentVersion().Authors.ToList();
        CreateNewContractAndAddRevision();
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts 
            .Select(c => new { 
                c.CurrentVersionId, 
                current = c.Versions.FirstOrDefault(v => v.Id == c.CurrentVersionId) 
            }).FirstOrDefault();
        var authorsv2 = contractFromDB.current.Authors.ToList();
        Assert.AreEqual(JsonSerializer.Serialize(authorsv1, CustomJsonOptions()),
            JsonSerializer.Serialize(authorsv2, CustomJsonOptions()));
    }

    private void CreateNewContractAndAddRevision()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
        var v1 = contractFromDB.CurrentVersion();
        var authorsCopy = new List<Author>();
        v1.Authors.ToList().ForEach(author => authorsCopy.Add(author.Copy()));
        contractFromDB.CreateRevisionUsingSameSpecs
            (ContractBC.Enums.ModReason.Other, "abc",
             v1.WorkingTitle, authorsCopy, null);
    }

    private void CreateNewContractAndAddRevisionWithNewSpecs()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
        var v1 = contractFromDB.CurrentVersion();
        var authors = new List<Author>();
        v1.Authors.ToList().ForEach(author => authors.Add(author.Copy()));
        contractFromDB.CreateRevisionUsingNewSpecs
            (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, 
             authors, null, SpecificationSet.Default());
    }

 
}
