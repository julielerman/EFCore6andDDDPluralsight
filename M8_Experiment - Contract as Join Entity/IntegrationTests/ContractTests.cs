
using AuthorBook.Infrastructure.Data;
using AuthorBookBC.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublisherSystem.SharedKernel.ValueObjects;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationTests;

[TestClass]
public class ContractTests
{
    AuthorBookContext _context;
    public ContractTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthorBookContext>();
        optionsBuilder.UseSqlServer(
             "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=AuthorBookIntegrationTests");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        var _options = optionsBuilder.Options;
        _context = new AuthorBookContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [TestMethod]
    public void CanCreateANewBookAndAuthorFromContract()
    {
        var authorname = new PersonName("Julie", "Lerman");
        var incomingContractId= Guid.NewGuid();
        var incomingContractDate= DateTime.UtcNow;
        var contract = new PubContract(authorname, "jlgmail", "111111", "my book", incomingContractId, incomingContractDate);
        _context.Add(contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractDB = _context.Contracts.Include(c => c.Author).Include(c => c.Book)
           .FirstOrDefault(c => c.PreCreatedContractId == incomingContractId);

         var expected = JsonSerializer.Serialize(contract.Author, CustomJsonOptions());
         var actual = JsonSerializer.Serialize(contractDB.Author, CustomJsonOptions());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CanCreateANewBookWithExistingAuthorFromContract()
    {
        var existingAuthor = new Author(new PersonName("Julie", "Lerman"), "jlgmail", "1111");
        _context.Add(existingAuthor);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var incomingContractId = Guid.NewGuid();
        var incomingContractDate = DateTime.UtcNow;
        var contract = new PubContract(existingAuthor.Id, "my book", incomingContractId, incomingContractDate);
        _context.Add(contract);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var contractWithBookDB = _context.Contracts.Include(c=>c.Book)
            .FirstOrDefault(c=>c.Author.Id==existingAuthor.Id && c.PreCreatedContractId==incomingContractId);

        var expected = JsonSerializer.Serialize(contract.Book, CustomJsonOptions()); ;
        var actual = JsonSerializer.Serialize(contractWithBookDB.Book, CustomJsonOptions());
        Assert.AreEqual(expected,actual );
    }

    [TestMethod]
    public void StoringNewCoAuthorsAndBookReturnsBothAuthorIds()
    {
        Guid incomingContractId ;
        PubContract contract1;
        PubContract contract2;
        StoreNewCoAuthorsAndBook(out incomingContractId, out contract1,out contract2);
        //var results = _context.Authors.Include(a => a.Contract.Book)
        //    .Where(a => a.Contract.PreCreatedContractId == incomingContractId).ToList();
        var results = _context.Authors.Select(a => a.Id).ToList();
        Assert.AreNotEqual(results[0], results[1]);

    }

    [TestMethod]
    public void StoringNewCoAuthorsAndBookReturnsTwoResultsWIthSameBookId()
    {
        Guid incomingContractId;
        PubContract contract1;
        PubContract contract2;
        StoreNewCoAuthorsAndBook(out incomingContractId, out contract1, out contract2);
        //var results = _context.Authors.Include(a => a.Contract.Book)
        //    .Where(a => a.Contract.PreCreatedContractId == incomingContractId).ToList();
        var results = _context.Contracts.Select(c => c.BookId).ToList();
        Assert.AreEqual(results[0], results[1]);

    }

    private void StoreNewCoAuthorsAndBook(out Guid incomingContractId, out PubContract c1, out PubContract c2)
    {
        var authorname1 = new PersonName("Julie", "Lerman");
        var authorname2 = new PersonName("Roland", "Guijt");
        incomingContractId = Guid.NewGuid();
        var incomingContractDate = DateTime.UtcNow;
        c1 = new PubContract(authorname1, "jlgmail", "111111", "our book", incomingContractId, incomingContractDate);
        c2 = c1.CreateCoAuthorContract(authorname2, "rgmail", "222222");

        _context.Add(c1);
        _context.Add(c2);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    [TestMethod]
    public void CoAuthorsEachHaveTwoBooks()
    {
        //PersistTwoBooksCoAuthoredByTwoAuthors();
        //_context.ChangeTracker.Clear();
        //var count1 = _context.Authors.Where(a => a.PreferredName == "firsty")
        //    .Select(a => a.Books.Count()).FirstOrDefault();
        //var count2= _context.Authors.Where(a => a.PreferredName == "firsty2")
        //    .Select(a => a.Books.Count()).FirstOrDefault();
        //CollectionAssert.AreEqual(new int[] { 2, 2 }, new int[] { count1, count2 });
    }

    [TestMethod]
    public void CoAuthoredBooksEachHaveTwoAuthors()
    {
        //PersistTwoBooksCoAuthoredByTwoAuthors();
        //_context.ChangeTracker.Clear();
        //var count1 = _context.Books.Where(b => b.Title == "some co-authored book")
        //    .Select(b=>b.AuthorIds.Count()).FirstOrDefault();
        //var count2 = _context.Books.Where(b => b.Title == "another co-authored book")
        //    .Select(b => b.AuthorIds.Count()).FirstOrDefault(); ;
        //  CollectionAssert.AreEqual(new int[] { 2, 2 }, new int[] { count1, count2 });
    }

    private void PersistTwoBooksCoAuthoredByTwoAuthors()
    {
        //var author1 = new Author(new PersonName("first1", "last1"),
        //    "firsty", "he/his", "firsty@firsty.com");
        //var author2 = new Author(new PersonName("first2", "last2"),
        //    "firsty2", "they/their", "firsty2@firsty2.com");
        //var book1 = new Book("some co-authored book", Guid.NewGuid(), DateTime.Now);
        //var book2 = new Book("another co-authored book", Guid.NewGuid(), DateTime.Now);
        //author1.AddExistingBook(book1);
        //author1.AddExistingBook(book2);
        //author2.AddExistingBook(book1);
        //author2.AddExistingBook(book2);

        //_context.Authors.AddRange(author1, author2);
        //_context.SaveChanges();
    }

    private JsonSerializerOptions CustomJsonOptions()
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
        options.Converters.Add(new CustomDecimalConverter("F"));
        options.ReferenceHandler = ReferenceHandler.Preserve;
        return options;

    }
    private JsonSerializerOptions CustomJsonOptionsIgnoreGraph()
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
        options.Converters.Add(new CustomDecimalConverter("F"));
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        return options;

    }
}