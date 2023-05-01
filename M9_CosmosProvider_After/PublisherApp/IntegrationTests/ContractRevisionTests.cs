using ContractBC.ContractAggregate;
using ContractBC.Enums;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
        optionsBuilder.UseCosmos("AccountEndpoint = ***endpointURLgoeshere***;AccountKey=*yours, not mine!*",
                                 "PublisherContractData");

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
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        Assert.AreEqual(2, contractFromDB.Versions.Count());
    }

    [TestMethod]
    public void RevisonUsingSameSpecsStoresHasRevisedAsFalse()
    {
         CreateNewContractAndAddRevision();
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contract = _context.Contracts.FirstOrDefault();
        var versionEntry = _context.Entry(contract.Versions.OrderBy(v=>v.DateCreated).FirstOrDefault());
        var jsonProperty = versionEntry.Property<JObject>("__jObject");
        var value = jsonProperty.CurrentValue["_hasRevisedSpecSet"];
        Assert.AreEqual(false, value);
    }
    [TestMethod]
    public void RevisonUsingNewSpecsStoresHasRevisedAsTrue()
    {
        CreateNewContractAndAddRevisionWithNewSpecs();
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contract = _context.Contracts.FirstOrDefault();
         var versionEntry = _context.Entry(contract.Versions.OrderByDescending(v => v.DateCreated)
             .FirstOrDefault());
        var jsonProperty = versionEntry.Property<JObject>("__jObject");
        var value = jsonProperty.CurrentValue["_hasRevisedSpecSet"];
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void PersistedContractWithRevisonAndSameAuthorsDuplicatesAuthorInDatabase()
    {
        var authorsv1 = _contract.CurrentVersion().Authors.ToList();
        CreateNewContractAndAddRevision();
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        //var contractFromDB = _context.Contracts 
        //    .Select(c => new { 
        //        c.CurrentVersionId, 
        //        current = c.Versions.FirstOrDefault(v => v.Id == c.CurrentVersionId) 
        //    }).FirstOrDefault();
        var contractFromDB=_context.Contracts.FirstOrDefault();
        var newVersion = 
            contractFromDB.Versions.FirstOrDefault(v => v.Id == contractFromDB.CurrentVersionId);
        var authorsv2 = newVersion.Authors.ToList();
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
