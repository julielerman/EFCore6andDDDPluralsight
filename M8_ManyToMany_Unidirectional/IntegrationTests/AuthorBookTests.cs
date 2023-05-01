using AuthorAndBookMaintenance.DomainModels;
using AuthorBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublisherSystem.SharedKernel.ValueObjects;

namespace IntegrationTests;

[TestClass]
public class AuthorBookTests
{
    AuthorBookContext _context;
    public AuthorBookTests()
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
    public void CanSerializeAndRetrieveBookAuthorIds()
    {   
        var authorId1= Guid.NewGuid();
        var authorId2= Guid.NewGuid();
        var listOfIds = new List<Guid> { authorId1, authorId2 };
        var book = new Book(listOfIds, "my book", Guid.NewGuid(), DateTime.Now);
        _context.Books.Add(book);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        var bookFromDB = _context.Books.FirstOrDefault();
        CollectionAssert.AreEqual(listOfIds, (System.Collections.ICollection)bookFromDB.AuthorIds);
    }

    [TestMethod]
    public void CoAuthorsEachHaveTwoBooks()
    {
        PersistTwoBooksCoAuthoredByTwoAuthors();
        _context.ChangeTracker.Clear();
        var count1 = _context.Authors.Where(a => a.PreferredName == "firsty")
            .Select(a => a.Books.Count()).FirstOrDefault();
        var count2= _context.Authors.Where(a => a.PreferredName == "firsty2")
            .Select(a => a.Books.Count()).FirstOrDefault();
        CollectionAssert.AreEqual(new int[] { 2, 2 }, new int[] { count1, count2 });
    }

    [TestMethod]
    public void CoAuthoredBooksEachHaveTwoAuthors()
    {
        PersistTwoBooksCoAuthoredByTwoAuthors();
        _context.ChangeTracker.Clear();
        var count1 = _context.Books.Where(b => b.Title == "some co-authored book")
            .Select(b=>b.AuthorIds.Count()).FirstOrDefault();
        var count2 = _context.Books.Where(b => b.Title == "another co-authored book")
            .Select(b => b.AuthorIds.Count()).FirstOrDefault(); ;
          CollectionAssert.AreEqual(new int[] { 2, 2 }, new int[] { count1, count2 });
    }

    private void PersistTwoBooksCoAuthoredByTwoAuthors()
    {
        var author1 = new Author(new PersonName("first1", "last1"),
            "firsty", "he/his", "firsty@firsty.com");
        var author2 = new Author(new PersonName("first2", "last2"),
            "firsty2", "they/their", "firsty2@firsty2.com");
        var book1 = new Book("some co-authored book", Guid.NewGuid(), DateTime.Now);
        var book2 = new Book("another co-authored book", Guid.NewGuid(), DateTime.Now);
        author1.AddExistingBook(book1);
        author1.AddExistingBook(book2);
        author2.AddExistingBook(book1);
        author2.AddExistingBook(book2);

        _context.Authors.AddRange(author1, author2);
        _context.SaveChanges();
    }
}