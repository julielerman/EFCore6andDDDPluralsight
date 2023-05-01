using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    [TestClass]
    public class QueryingTests
    {
        Contract _contract = new Contract(DateTime.Today,
              new List<Author> { Author.UnsignedAuthor("first", "last", "email", "phone") },
              "booktitle");
        ContractContext _context;

        public QueryingTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
            optionsBuilder.UseCosmos("AccountEndpoint = ***endpointURLgoeshere***;AccountKey=*yours, not mine!*",
                               "PublisherContractData");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            var _options = optionsBuilder.Options;
            _context = new ContractContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
     
        [TestMethod]
        public void CanIncludeRelatedDatabyDefault()
        {
         //succeeds...embedded data is treated as owned and always retrieved   
            var assignedId = _contract.Id;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contract = _context.Contracts.FirstOrDefault();
            Assert.AreEqual(1, contract.Versions.Count());
        }
        [TestMethod]
        public void CanExcludeRelatedData()
        {
            //fails...embedded data is treated as owned and always retrieved   
            var assignedId = _contract.Id;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contract = _context.Contracts.Select(c=>c).FirstOrDefault();
            Assert.AreEqual(0, contract.Versions.Count());
        }
        [TestMethod]
        public void CanUseFilteredInclude()
        {
            //fails...filtered include is not supported   
            var vid = _contract.Versions.FirstOrDefault().Id;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contract = _context.Contracts.Include(c=>c.Versions.Where(v=>v.Id==vid)).FirstOrDefault();
            Assert.AreEqual(1, contract.Versions.Count());
        }
        [TestMethod]
        public void CanProjectRootProperties()
        {   //succeeds
            var assignedId = _contract.Id;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var idFromDB = _context.Contracts.Select(c => c.Id).FirstOrDefault();
            Assert.AreEqual(assignedId, idFromDB);
        }
        [TestMethod]
        public void CanProjectOwnedCollectionsWithNoTracking()
        {
            //This works
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var versions = _context.Contracts.AsNoTracking().Select(c => c.Versions).FirstOrDefault();
            Assert.AreEqual(1, versions.Count());
        }
        [TestMethod]
        public void CanProjectOwnedCollectionsWithDefaultTracking()
        { //this fails...the embedded objects need root for tracking

            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var versions = _context.Contracts.Select(c => c.Versions).FirstOrDefault();
            Assert.AreEqual(1, versions.Count());
        }
        [TestMethod]
        public void CanFilterProjectionOfOwnedCollectionsWithNoTracking()
        {
            //this fails...the projection filter can't be translated
            var cvid = _contract.CurrentVersionId;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var versions = _context.Contracts.AsNoTracking()
                .Select(c => c.Versions.Where(v=>v.Id==cvid)).FirstOrDefault();
            Assert.AreEqual(1, versions.Count());
        }
        [TestMethod]
        public void CanAggregateProjectionOfOwnedCollections()
        {
            //this fails...the aggregate isn't supported
            var cvid = _contract.CurrentVersionId;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var versionCount = _context.Contracts
                .Select(c => c.Versions.Count()).FirstOrDefault();
            Assert.AreEqual(1, versionCount);
        }
    }
}
