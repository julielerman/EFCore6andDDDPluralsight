using ContractBC.ValueObjects;
using PublisherSystem.SharedKernel.DTOs;

namespace ContractBC.UnitTests;

[TestClass]
public class AuthorTests
{
    [TestMethod]
    public void CanCreateSignedAuthor()
    {  var signedId=Guid.NewGuid();
        var author = Author.SignedAuthor("first", "last","email","phone",signedId); ;
        CollectionAssert.AreEqual(new object[] { true, signedId }, new object[] {author.Signed,author.SignedAuthorId});
    }
    [TestMethod]
    public void CanCreateUnsignedAuthor()
    {
        var author = Author.UnsignedAuthor( "first", "last","email", "phone");
        Assert.AreEqual(false, author.Signed);
    }
    [TestMethod]
    public void CanCreateANewAuthorViaFixAuthorName()
    {
        var author = Author.UnsignedAuthor( "first", "last", "email", "phone");
        var newauthor=author.FixName("newfirst", "newlast");
        CollectionAssert.AreEquivalent(
            new string[] { "newfirst newlast", "email", "phone" },
            new string[] { newauthor.FullName, newauthor.Email, newauthor.Phone });
    }
    [TestMethod]
    public void CanCreateANewAuthorViaPhone()
    {
        var author = Author.UnsignedAuthor("first", "last", "email",string.Empty);
        var newauthor = author.AddPhone("111111");
        CollectionAssert.AreEquivalent(
            new string[] { "first last", "email", "111111" },
            new string[] { newauthor.FullName, newauthor.Email, newauthor.Phone });
    }
    [TestMethod]
    public void UnsignedAuthorHasNoSignedId() {
        var author = Author.UnsignedAuthor( "first", "last", "email", "phone");
        Assert.AreEqual(Guid.Empty, author.SignedAuthorId);
    }
   

}