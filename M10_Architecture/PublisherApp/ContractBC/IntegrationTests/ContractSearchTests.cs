using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntegrationTests
{
    [TestClass]
    public class ContractSearchTests
    {
        SearchContext _context;
        ContractSearchService _repo;
        public ContractSearchTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SearchContext>();
            optionsBuilder.UseSqlServer(
                 "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubServiceTests");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
            var _options = optionsBuilder.Options;
            _context = new SearchContext(_options);
             Seed();
            _repo = new ContractSearchService(_context);
        }

        [TestMethod]
        public void ContractAuthorSearchFindsOneMatch()
        {
            var contractList = _repo.GetContractPickListForAuthorLastName("Ler").Result;
            Assert.AreEqual(1, contractList.Count());
        }

        [TestMethod]
        public void ContractAuthorSearchFindsTwoMatches()
        {
            var contractList = _repo.GetContractPickListForAuthorLastName("Le").Result;
            Assert.AreEqual(2, contractList.Count());
        }
        private void Seed()
        {
            var c1 = new Contract(
        DateTime.Today.AddDays(-10), new List<Author> { Author.UnsignedAuthor("Stonna", "Edelman", "s@stonna.com", "800-stonna") }, "A Book"
        );

            var c2 = new Contract(
DateTime.Today.AddDays(-2), new List<Author> { Author.UnsignedAuthor("Julie", "Lerman", "j@julie.com", "800-julie") }, "Another Book"
);
            c2.AddAuthor(Author.UnsignedAuthor("Roland", "Guijt", "r@roland.com", "800-roland"));
            var c3 = new Contract(
DateTime.Today, new List<Author> { Author.UnsignedAuthor("Liz", "Lemon", "l@liz.com", "800-liz") }, "Book II"
);
            var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
            optionsBuilder.UseSqlServer(
                 "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubServiceTests");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
            var contractContext = new ContractContext(optionsBuilder.Options);
            contractContext.Database.EnsureDeleted();
            contractContext.Database.Migrate();
            contractContext.AddRange(c1, c2, c3);
            contractContext.SaveChanges();

        }


    }
}