 using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IntegrationTests;

[TestClass]
public class ContractTests
{
    Contract _contract = new Contract(
        DateTime.Today,
        new List<Author> { Author.UnsignedAuthor("first", "last", "email", "phone") },
        "booktitle");
    ContractContext _context;

    public ContractTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
        optionsBuilder.UseSqlServer(
             "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubIntegrationTests");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();
        var _options = optionsBuilder.Options;
        _context = new ContractContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [TestMethod]
    public void NewContractStoresCorrectId()
    {
        var assignedId = _contract.Id;
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        Assert.AreEqual(assignedId, contractFromDB.Id);
    }

    [TestMethod]
    public void NewContractHasVersionWithSpecDefaults()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var defaultSpecs = SpecificationSet.Default();
        var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
        Assert.AreEqual(defaultSpecs, contractFromDB.Versions.First().Specs);
    }

    //Test properties that required configuration
    [TestMethod]
    public void NewContractHasContractNumberWhenQueried()
    {
        var calculatedContractNumber = _contract.ContractNumber;
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        Assert.AreEqual(calculatedContractNumber, contractFromDB.ContractNumber);
    }

    [TestMethod]
    public void NewContractHasDateInitiatedWhenQueried()
    {
        var calculatedDateInit = _contract.DateInitiated;
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        Assert.AreEqual(calculatedDateInit, contractFromDB.DateInitiated);
    }

    [TestMethod]
    public void HasRevisedSpecSetIsFalseForNewContractNewVersion()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var value = _context.Set<ContractVersion>()
            .Select(v => EF.Property<bool>(v, "_hasRevisedSpecSet")).FirstOrDefault();
        Assert.AreEqual(false, value);
    }

    //[TestMethod]
    //public void EFCore7_HasRevisedSpecSetIsFalseForNewContractNewVersion()
    //{
    //    _context.Contracts.Add(_contract);
    //    _context.SaveChanges();
    //    _context.ChangeTracker.Clear();
    //    var value = _context.Database
    //           .SqlQuery<bool>($"SELECT [_hasRevisedSpecSet] as [Value] FROM [ContractVersions]")
    //           .FirstOrDefault();
    //    Assert.AreEqual(false, value);
    //}
 
    //Full Graph using JSON
    [TestMethod]
    public void FullContractAggregateValuesPersistedandRetrieved()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts
            .Include(c => c.Versions).ThenInclude(v => v.Specs).FirstOrDefault();
        //Note: to force the test to fail, you can make the following modification, changing one of the contract instances
        //contractFromDB.FinalVersionSignedByAllParties();
        var expected = JsonSerializer.Serialize(_contract,CustomJsonOptions());
        var actual = JsonSerializer.Serialize(contractFromDB,CustomJsonOptions());
        Assert.AreEqual(expected, actual);
    }

    private JsonSerializerOptions CustomJsonOptions()
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
        options.Converters.Add(new CustomDecimalConverter("F"));
        return options;

    }

}


