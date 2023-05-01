using ContractBC.ValueObjects;

namespace ContractBC.UnitTests;

[TestClass]
public class NameTests
    {
        [TestMethod]
        public void CanGenerateComplexInitsFromLongFirstAndLastName()
        {
            var author = Author.UnsignedAuthor( "Julie", "Lerman","email", "phone");
            Assert.AreEqual("JulLer", author.Name.ComplexInitials);
        }

        [TestMethod]
        public void CanGenerateComplexInitsFromShortLastName()
        {
            var author = Author.UnsignedAuthor( "Celeste", "Ng", "email", "phone");
            Assert.AreEqual("CelNg_", author.Name.ComplexInitials);
        }

        [TestMethod]
        public void CanGenerateComplexInitsFromShortFirstName()
        {
            var author = Author.UnsignedAuthor( "Mo", "Rocca", "email", "phone");
            Assert.AreEqual("Mo_Roc", author.Name.ComplexInitials);
        }
    }

