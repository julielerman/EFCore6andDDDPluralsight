
using AuthorAndBookMaintenance.DomainModels;
using PublisherSystem.SharedKernel.ValueObjects;
using System.Diagnostics;
using System.Text.Json;

namespace UnitTests;

[TestClass]
public class AuthorBookTests
{
    [TestMethod]
    public void CanCreateNewBookWithAuthorId()
    {
        Guid authorId= Guid.NewGuid();
        var book = new Book(authorId, "title", Genres.Fantasy, FictionNonFiction.Fiction);
        CollectionAssert.AreEqual(
           new object[] {authorId,"title",Genres.Fantasy,FictionNonFiction.Fiction},
           new object[] { book.AuthorIds.First(), book.Title, book.PrimaryGenre, book.FictionOrNonFiction });
    }
    [TestMethod]
    public void CanAddAuthorIdToBook()
    {
        Guid authorId1 = Guid.NewGuid();
        Guid authorId2 = Guid.NewGuid();
        var book = new Book(authorId1,"title", Genres.Fantasy, FictionNonFiction.Fiction);
        book.AddAuthorId(authorId2);
        Assert.IsTrue(book.AuthorIds.Contains(authorId2));
    }


}