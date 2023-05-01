using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IntegrationTests;

[TestClass]
public class ContractServiceTests
{
    Contract _contract = new Contract(
       DateTime.Today, new List<Author> { Author.UnsignedAuthor("first", "last", "email", "phone") }, "booktitle"
       );
    ContractContext _context;
    ContractServices _service;
    public ContractServiceTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
        optionsBuilder.UseSqlServer(
             "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubRepoTests");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        var _options = optionsBuilder.Options;
        _context = new ContractContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _service = new ContractServices(_context);
    }

    [TestMethod]
    public void CanGetAuthorsForContract()
    {
        var cid = _contract.Id;
        _contract.CurrentVersion().AddAuthor(Author.UnsignedAuthor("Julie", "Lerman", "jl@jl", "111"));
        _context.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var authors = _service.GetAuthorsFromAllVersionsOfAContract(cid);
        Assert.AreEqual(2, authors.Count);
    }

    [TestMethod]
    public void CanSaveNewContractViaRepo()
    {//note testing the repo Add method, so query is directly via context
        var cid = _contract.Id;
        _service.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var idFromDB = _context.Contracts.Select(c => c.Id).SingleOrDefault();
        Assert.AreEqual(cid, idFromDB);
    }

    [TestMethod]
    public void CanRetrieveNewContractViaRepo()
    {
        var cid = _contract.Id;
        _context.Add(_contract); //intentionally using context, not repo
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _service.GetContractAggregateById(cid);
        var expected = JsonSerializer.Serialize(_contract, CustomJsonOptions());
        var actual = JsonSerializer.Serialize(contractFromDB, CustomJsonOptions());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CanAcceptVersionViaService()
    {
        var cid = _contract.Id;
        _service.Add(_contract);
        _context.ChangeTracker.Clear();    
        _service.AcceptCurrentVersion(cid);
        var contractFromDB = _service.GetContractWithCurrentVersionOnly (cid);
        Assert.IsTrue(contractFromDB.CurrentVersion().Accepted);
    }

    [TestMethod]
    public void AddRevisionUpdatesVersionCount()
    {
        var contractWithUnsavedRevision = CreateNewContractAndAddRevision();
        _context.ChangeTracker.Clear();
        _service.AddRevision(contractWithUnsavedRevision);
        _context.ChangeTracker.Clear();
        var contract = _service.GetContractAggregateById(_contract.Id);
        Assert.AreEqual(2, contract.Versions.Count());
    }

    [TestMethod]
    public void CanAddAuthorToCurrentVersion()
    {
        //Arrange
        var cid = _contract.Id;
        _service.Add(_contract);
        _context.ChangeTracker.Clear();
        var cFromDB=_service.GetContractWithCurrentVersionOnly(cid);
        //Act
        cFromDB.AddAuthor(Author.UnsignedAuthor("Kim", "Karnatz", "kim@kim.kim", "1800Kim"));
        _service.Update(cFromDB);
        _context.ChangeTracker.Clear(); 
        //Assert
        var newCFromDB= _service.GetContractWithCurrentVersionOnly(cid);
        Assert.AreEqual(2, newCFromDB.CurrentVersion().Authors.Count());
    }

    [TestMethod]
    public void CanRetrieveContractThatHasMultipleVersionsWithOnlyOneVersion()
    {
        var contractWithUnsavedRevision=CreateNewContractAndAddRevision();
        _service.AddRevision(contractWithUnsavedRevision);
        _context.ChangeTracker.Clear();
        var storedContract=_service.GetContractWithCurrentVersionOnly(_contract.Id);
        Assert.AreEqual(1,storedContract.Versions.Count());

    }

   

    [TestMethod]
    public void CanAddAuthorToCurrentVersionViaRepo()
    {
        _service.Add(_contract);
        _context.ChangeTracker.Clear();
        var contractFromDB = _service.GetContractAggregateById(_contract.Id);
           contractFromDB.AddAuthor(Author.UnsignedAuthor("Alex", "Walton", "", ""));
        _service.Update(contractFromDB);
        _context.ChangeTracker.Clear();
        var contractFromDb = _service.GetContractWithCurrentVersionOnly(_contract.Id);
        Assert.AreEqual(2, contractFromDb.CurrentVersion().Authors.Count());
    }

    private JsonSerializerOptions CustomJsonOptions()
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
        options.Converters.Add(new CustomDecimalConverter("F"));
        return options;
    }

    private Contract CreateNewContractAndAddRevision()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
        var v1 = contractFromDB.CurrentVersion();
        var authors = new List<Author>();
        v1.Authors.ToList().ForEach(author => authors.Add(author.Copy()));
        contractFromDB.CreateRevisionUsingSameSpecs
          (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, authors, null);
        return contractFromDB;
    }
}