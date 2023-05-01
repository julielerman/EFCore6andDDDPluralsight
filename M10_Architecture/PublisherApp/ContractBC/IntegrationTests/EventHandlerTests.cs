using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntegrationTests;

[TestClass]
public class EventHandlerTests
{
    Contract _contract = new Contract(
       DateTime.Today, new List<Author> { Author.UnsignedAuthor("first", "last","email","phone") },
       "booktitle" );
    ContractContext _context;

    public EventHandlerTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
        optionsBuilder.UseSqlServer(
             "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubContractTests");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        var mockMediator = new Mock<IMediator>();

        var _options = optionsBuilder.Options;
        var dbSetupContext = new ContractContext(_options);
        dbSetupContext.Database.EnsureDeleted();
        dbSetupContext.Database.EnsureCreated();
        dbSetupContext.Dispose();
        _context = new ContractContext(_options, mockMediator.Object);
    }
    [TestMethod]
    public void SignedContractCreatesEvent()
    {
        _contract.FinalVersionSignedByAllParties();
        Assert.AreEqual(1, _contract.Events.Count);
    }

    [TestMethod]
    public void SavingSignedContractTriggersHandlerWhichRemovesEvent()
    {
        _contract.FinalVersionSignedByAllParties();
        _context.Contracts.Add(_contract);
       var result= _context.SaveChanges();
        Assert.AreEqual(0, _contract.Events.Count);
    }

    private void CreateNewContractAndAddRevision()
    {
        _context.Contracts.Add(_contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
        var v1 = contractFromDB.CurrentVersion();
        var authors = new List<Author>();
        v1.Authors.ToList().ForEach(author => authors.Add(author.Copy()));
        //contractFromDB.CreateRevisionUsingSameSpecs
        //    (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, v1.Authors.ToList(), null);
        contractFromDB.CreateRevisionUsingSameSpecs
            (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, authors, null);

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
            (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, authors, null,SpecificationSet.Default());

    }
}