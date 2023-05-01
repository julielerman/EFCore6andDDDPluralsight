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
        optionsBuilder.UseCosmos(
            "AccountEndpoint = ***endpointURLgoeshere***;AccountKey=*yours, not mine!*",
            "PublisherContractData"); 
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
        var contractFromDB = _context.Contracts.FirstOrDefault();
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
        var contract = _context.Contracts.FirstOrDefault();
        var versionEntry = _context.Entry(contract.Versions.FirstOrDefault());
        var jsonProperty = versionEntry.Property<JObject>("__jObject");
        var value=jsonProperty.CurrentValue["_hasRevisedSpecSet"] ;
        Assert.AreEqual(false, value);
    }

 
    //Full Graph using JSON
    [TestMethod]
    public void FullContractAggregateValuesPersistedandRetrieved()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        //Note: to force the test to fail, you can make the following modification, changing one of the contract instances
        //contractFromDB.FinalVersionSignedByAllParties();
        var expected = JsonSerializer.Serialize(_contract,CustomJsonOptions());
        var actual = JsonSerializer.Serialize(contractFromDB,CustomJsonOptions());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CanPersistAuthorSocialMediaDictionary()
    {
        var author = Author.UnsignedAuthor("Julie", "Lerman", "julie@boo.boo", "111111");
        author.AddSocialMedia(SocialMedia.Twitter, "julielerman");
        author.AddSocialMedia(SocialMedia.Mastadon, "@julielerman@mas.to");
        var contract = new Contract(DateTime.Today, new List<Author> { author }, "I wrote a book!");
        _context.Contracts.Add(contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        var expected = new Dictionary<string, string>() { { "Twitter", "julielerman" }, { "Mastadon", "@julielerman@mas.to" } };
        var authorFromDB = contractFromDB.CurrentVersion().Authors.FirstOrDefault();
        CollectionAssert.AreEqual(expected, authorFromDB.SocialMediaAccounts);
        }

    [TestMethod]
    public void CanPersistContractComments()
    {
        _contract.AddNewComments("number one");
        _contract.AddNewComments("number two");
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        CollectionAssert.AreEqual(_contract.Comments, contractFromDB.Comments);
    }

    [TestMethod]
    public void FullContractAggregateWithSocialMediaAndCommentsPersistedandRetrieved()
    {
        var author = Author.UnsignedAuthor("Julie", "Lerman", "julie@boo.boo", "111111");
        author.AddSocialMedia(SocialMedia.Twitter, "julielerman");
        author.AddSocialMedia(SocialMedia.Mastadon, "@julielerman@mas.to");
        var contract = new Contract(DateTime.Today, new List<Author> { author }, "I wrote a book!");
        contract.AddNewComments("number one");
        contract.AddNewComments("number two");
        _context.Contracts.Add(contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.FirstOrDefault();
        //Note: to force the test to fail, you can make the following modification, changing one of the contract instances
        //contractFromDB.FinalVersionSignedByAllParties();
        var expected = JsonSerializer.Serialize(contract, CustomJsonOptions());
        var actual = JsonSerializer.Serialize(contractFromDB, CustomJsonOptions());
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


